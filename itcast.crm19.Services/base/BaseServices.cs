using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.Services
{
    using itcast.crm19.model;
    using itcast.crm19.IServices;
    using System.Linq.Expressions;
    using itcast.crm19.IRepository;


    /// <summary>
    /// 负责操作所有表的统一的业务逻辑
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BaseServices<TEntity> : IBaseServices<TEntity> where TEntity : class
    {
        protected IBaseDal<TEntity> baseDal;

        #region 1.0 查询

        #region 1.0.1 带条件查询

        public IQueryable<TEntity> QueryWhere(Expression<Func<TEntity, bool>> where)
        {
            return baseDal.QueryWhere(where);
        }


        #endregion

        #region 1.0.2 连表带条件查询

        public IQueryable<TEntity> QueryJoin(Expression<Func<TEntity, bool>> where, string[] tableNames)
        {
            return baseDal.QueryJoin(where, tableNames);
        }

        #endregion

        #region 1.0.3 带条件的分页查询

        public IQueryable<TEntity> QueryByPage<TKey>(int pageIndex
            , int pageSize
            , out int totalCount
            , Expression<Func<TEntity, bool>> where
            , Expression<Func<TEntity, TKey>> orderby)
        {
            return baseDal.QueryByPage(pageIndex, pageSize, out totalCount, where, orderby);
        }

        #endregion


        #endregion

        #region 2.0 新增

        public void Add(TEntity model)
        {
            baseDal.Add(model);
        }


        #endregion

        #region 3.0 自定义实体的按需编辑

        public void Edit(TEntity model, string[] propertyNames)
        {
            baseDal.Edit(model, propertyNames);
        }


        #endregion

        #region 4.0 物理删除

        public void Delete(TEntity model, bool isAddedDbContext)
        {
            baseDal.Delete(model, isAddedDbContext);
        }

        #endregion

        #region 5.0 统一保存

        public int SaveChanges()
        {
            return baseDal.SaveChanges();
        }

        #endregion



        public List<TElement> RunProc<TElement>(string sql, params object[] parms)
        {
            return baseDal.RunProc<TElement>(sql, parms);
        }
    }
}
