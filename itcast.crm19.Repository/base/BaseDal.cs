using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.Repository
{
    using System.Linq.Expressions;
    using itcast.crm19.IRepository;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Runtime.Remoting.Messaging;

    /// <summary>
    /// 负责调用EF容器对象实例操作所有表的增，删，查，改
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseDal<TEntity> : IBaseDal<TEntity> where TEntity : class
    {
        //1.0 实例化EF上下文容器对象
        //缺点：在处理线程中根据控制器的构造函数参数的个数就会创建相同个数的EF容器对象，导致每个操作只能保证一部分业务可以影响到数据库
        //降低系统性能，和容易出现bug
        //BaseDbContext db = new BaseDbContext();

        //解决上述问题的方案：保证每个http的处理线程中只有一份EF容器对象
        public BaseDbContext db
        {
            get
            {
                //1.0 问线程缓存对象要EF容器对象，如果返回为null，则创建ef容器对象，同时保存到线程缓存对象中
                object efcontextInstance = CallContext.GetData("BaseDbContext");
                if (efcontextInstance == null)
                {
                    efcontextInstance = new BaseDbContext();

                    //保存到线程缓存对象中
                    CallContext.SetData("BaseDbContext", efcontextInstance);
                }

                return efcontextInstance as BaseDbContext;
            }
        }

        //2.0 定义一个私有的DbSet<TEntity> 字段
        DbSet<TEntity> _dbset;
        /// <summary>
        /// 初始化_dbset
        /// </summary>
        public BaseDal()
        {
            _dbset = db.Set<TEntity>();
        }

        #region 1.0 查询

        #region 1.0.1 带条件查询

        public IQueryable<TEntity> QueryWhere(Expression<Func<TEntity, bool>> where)
        {
            return _dbset.Where(where);
        }


        #endregion

        #region 1.0.2 连表带条件查询

        public IQueryable<TEntity> QueryJoin(Expression<Func<TEntity, bool>> where, string[] tableNames)
        {
            //1.0 参数合法性验证
            if (tableNames == null || tableNames.Any() == false)
            {
                throw new Exception("至少要有一个表");
            }

            //2.0 遍历tableNames
            DbQuery<TEntity> query = _dbset;
            foreach (var tableName in tableNames)
            {
                query = query.Include(tableName);
            }

            //3.0 条件连表查询
            return query.Where(where);
        }

        #endregion

        #region 1.0.3 带条件的分页查询

        public IQueryable<TEntity> QueryByPage<TKey>(int pageIndex
            , int pageSize
            , out int totalCount
            , Expression<Func<TEntity, bool>> where
            , Expression<Func<TEntity, TKey>> orderby)
        {
            //计算出要跳过的总条数
            int skipCount = (pageIndex - 1) * pageSize;

            //2.0 给totalCount 赋值
            totalCount = _dbset.Count(where);

            //3.0 返回分页数据
            return _dbset.Where(where).OrderByDescending(orderby).Skip(skipCount).Take(pageSize);
        }

        #endregion

        #region 1.0.4 统一调用存储过程

        /// <summary>
        /// 此方法既可以执行sql语句也可以执行存储过程
        /// 执行存储过程的写法:
        /// sql参数传入的是 存储过程名称 空格 参数1,参数2
        ///  dbo.Usp_GetPermissMenu19 8
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public List<TElement> RunProc<TElement>(string sql, params object[] parms)
        {
            return db.Database.SqlQuery<TElement>(sql, parms).ToList();
        }

        #endregion


        #endregion

        #region 2.0 新增

        public void Add(TEntity model)
        {
            //1.0 参数检查
            if (model == null)
            {
                throw new Exception("实体model不能为空");
            }

            //2.0 将model追加到EF容器中同时将其状态修改成Added
            _dbset.Add(model);
        }


        #endregion

        #region 3.0 自定义实体的按需编辑

        public void Edit(TEntity model, string[] propertyNames)
        {
            //合法性检查
            if (model == null)
            {
                throw new Exception("实体model不能为空");
            }

            if (propertyNames == null || propertyNames.Any() == false)
            {
                throw new Exception("被修改的属性至少有一个");
            }

            //2.0 将model追加到EF容器中
            var entry = db.Entry(model);
            entry.State = System.Data.EntityState.Unchanged;

            //3.0 将需要修改的属性的IsModified修改成true
            foreach (var propertyName in propertyNames)
            {
                entry.Property(propertyName).IsModified = true;
            }

            //4.0 关闭EF的实体属性合法性检查
            db.Configuration.ValidateOnSaveEnabled = false;
        }


        #endregion

        #region 4.0 物理删除

        public void Delete(TEntity model, bool isAddedDbContext)
        {
            if (isAddedDbContext == false)
            {
                _dbset.Attach(model);
            }

            _dbset.Remove(model); //将model代理类的状态修改成Deleted
        }

        #endregion

        #region 5.0 统一保存

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        #endregion
    }
}
