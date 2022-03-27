using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Data.Access.Interface
{

    public interface IBaseBLL { }

    [ServiceContract(Namespace = "Data.Access.BLL")]
    public interface IBaseBLL<T> : IBaseBLL
    {
        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "GetHistoryData")]
        List<T> GetHistoryData(DateTime from, DateTime to, Guid? deviceKey);

        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "ListModels")]
        List<T> ListModels(int pageSize, int index);

        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "GetModelCount")]
        int GetModelCount();

        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "Save")]
        int Save(T obj);

        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "Update")]
        void Update(T obj);

        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "Get")]
        T Get(T obj);

        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "Get")]
        T GetBySourceId(string sourceId);

        [FaultContract(typeof(Exception))]
        [OperationContract(Name = "Delete")]
        void Delete(T obj);
    }




}
