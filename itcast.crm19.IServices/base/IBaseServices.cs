using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace itcast.crm19.IServices
{
    public interface IBaseServices<TEntity> where TEntity : class
    {
        #region 1.0 查询

        #region 1.0.1 带条件查询

        IQueryable<TEntity> QueryWhere(Expression<Func<TEntity, bool>> where);


        #endregion

        #region 1.0.2 连表带条件查询

        IQueryable<TEntity> QueryJoin(Expression<Func<TEntity, bool>> where, string[] tableNames);

        #endregion

        #region 1.0.3 带条件的分页查询

        IQueryable<TEntity> QueryByPage<TKey>(int pageIndex
           , int pageSize
           , out int totalCount
           , Expression<Func<TEntity, bool>> where
           , Expression<Func<TEntity, TKey>> orderby);

        #endregion
        List<TElement> RunProc<TElement>(string sql, params object[] parms);

        #endregion

        #region 2.0 新增

        void Add(TEntity model);


        #endregion

        #region 3.0 自定义实体的按需编辑

        void Edit(TEntity model, string[] propertyNames);


        #endregion

        #region 4.0 物理删除

        void Delete(TEntity model, bool isAddedDbContext);

        #endregion

        #region 5.0 统一保存

        int SaveChanges();

        #endregion
    }
}
