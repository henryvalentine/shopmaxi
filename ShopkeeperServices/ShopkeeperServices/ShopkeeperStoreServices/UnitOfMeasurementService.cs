using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class UnitOfMeasurementServices
	{
		private readonly UnitOfMeasurementRepository  _unitsOfMeasurementRepository;
        public UnitOfMeasurementServices()
		{
            _unitsOfMeasurementRepository = new UnitOfMeasurementRepository();
		}
        public long AddUnitOfMeasurement(UnitsOfMeasurementObject unitsOfMeasurement)
		{
			try
			{
                return _unitsOfMeasurementRepository.AddUnitsOfMeasurement(unitsOfMeasurement);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}
        public UnitsOfMeasurementObject GetUnitsOfMeasurement(long unitsOfMeasurementId)
        {
            try
            {
                return _unitsOfMeasurementRepository.GetUnitOfMeasurement(unitsOfMeasurementId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new UnitsOfMeasurementObject();
            }
        }
        public int GetObjectCount()
        {
            try
            {
                return _unitsOfMeasurementRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        public int UpdateUnitOfMeasurement(UnitsOfMeasurementObject unitsOfMeasurement)
		{
			try
			{
                return _unitsOfMeasurementRepository.UpdateUnitsOfMeasurement(unitsOfMeasurement);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}
        public bool DeleteUnitOfMeasurement(long unitsOfMeasurementId)
		{
			try
			{
                return _unitsOfMeasurementRepository.DeleteUnitsOfMeasurement(unitsOfMeasurementId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}
        public UnitsOfMeasurementObject GetUnitOfMeasurement(long unitOfMeasurementId)
		{
			try
			{
                return _unitsOfMeasurementRepository.GetUnitOfMeasurement(unitOfMeasurementId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new UnitsOfMeasurementObject();
			}
		}
        public List<UnitsOfMeasurementObject> GetUnitsOfMeasurementObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _unitsOfMeasurementRepository.GetUnitsOfMeasurementObject(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<UnitsOfMeasurementObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UnitsOfMeasurementObject>();
            }
        }

        public List<UnitsOfMeasurementObject> GetUnitsofMeasurement()
        {
            try
            {
                var objList = _unitsOfMeasurementRepository.GetUnitsofMeasurement();
                if (objList == null || !objList.Any())
                {
                    return new List<UnitsOfMeasurementObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UnitsOfMeasurementObject>();
            }
        }
        public List<UnitsOfMeasurementObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _unitsOfMeasurementRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<UnitsOfMeasurementObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<UnitsOfMeasurementObject>();
            }
        }
	  }

}
