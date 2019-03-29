using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class StoreMessageServices
	{
        private readonly StoreMessageRepository _messageManager;
        public StoreMessageServices()
		{
            _messageManager = new StoreMessageRepository();
		}

        public long AddMessage(StoreMessageObject message)
		{
			try
			{
                return _messageManager.AddMessage(message);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public long AddMessagePerm(StoreMessageObject message)
        {
            try
            {
                return _messageManager.AddMessagePerm(message);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long DeleteMessage(long messageId)
        {
            try
            {
                return _messageManager.DeleteMessage(messageId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long UpdateMessage(StoreMessageObject message)
        {
            try
            {
                return _messageManager.UpdateMessage(message);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public List<StoreMessageObject> GetMessages()
		{
			try
			{
                var objList = _messageManager.GetMessages();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreMessageObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreMessageObject>();
			}
		}

        public StoreMessageObject GetMessage(long messageId)
        {
            try
            {
                return _messageManager.GetMessage(messageId);
                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageObject();
            }
        }

        public List<StoreMessageObject> GetMessages(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                var objList = _messageManager.GetMessages(itemsPerPage, pageNumber, out countG);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreMessageObject>();
			    }
                
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                countG = 0;
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> GetMyMessages(int? itemsPerPage, int? pageNumber, out int countG, long userId)
        {
            try
            {
                return _messageManager.GetMyMessages(itemsPerPage, pageNumber, out countG, userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                countG = 0;
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> GetMyLatestMessages(long userId)
        {
            try
            {
                return _messageManager.GetMyLatestMessages(userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> SearchMessages(string searchCriteria, long userId)
        {
            try
            {
                return _messageManager.SearchMessages(searchCriteria, userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> GetMyMessages(long userId)
        {
            try
            {
                return _messageManager.GetMyMessages(userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _messageManager.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreMessageObject>();
                }

                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreMessageObject>();
            }
        }
	}

}
