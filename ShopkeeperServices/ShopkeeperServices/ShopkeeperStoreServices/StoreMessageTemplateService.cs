using System;
using System.Collections.Generic;
using System.Linq;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperStoreServices
{
	public class StoreMessageTemplateServices
	{
        private readonly StoreMessageTemplateRepository _messageTemplateManager;
        public StoreMessageTemplateServices()
		{
            _messageTemplateManager = new StoreMessageTemplateRepository();
		}

        public long AddMessageTemplate(StoreMessageTemplateObject messageTemplate)
		{
			try
			{
                return _messageTemplateManager.AddMessageTemplate(messageTemplate);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public long DeleteMessageTemplate(long messageTemplateId)
        {
            try
            {
                return _messageTemplateManager.DeleteMessageTemplate(messageTemplateId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long UpdateMessageTemplate(StoreMessageTemplateObject messageTemplate)
        {
            try
            {
                return _messageTemplateManager.UpdateMessageTemplate(messageTemplate);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<StoreMessageTemplateObject> GetMessageTemplates()
		{
			try
			{
                var objList = _messageTemplateManager.GetMessageTemplates();
                if (objList == null || !objList.Any())
			    {
                    return new List<StoreMessageTemplateObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreMessageTemplateObject>();
			}
		}

        public StoreMessageTemplateObject GetMessageTemplate(long messageTemplateId)
        {
            try
            {
                return _messageTemplateManager.GetMessageTemplate(messageTemplateId);
                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageTemplateObject();
            }
        }

        public StoreMessageTemplateObject GetMessageTemp(int messageEventId)
        {
            try
            {
                return _messageTemplateManager.GetMessageTemp(messageEventId);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageTemplateObject();
            }
        }

        public StoreMessageTemplateObject GetMessageTemp(int messageEventId, string email)
        {
            try
            {
                return _messageTemplateManager.GetMessageTemp(messageEventId, email);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageTemplateObject();
            }
        }

        public StoreMessageTemplateObject GetMessageTempWithExpiry(int messageEventId)
        {
            try
            {
                return _messageTemplateManager.GetMessageTempWithExpiry(messageEventId);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageTemplateObject();
            }
        }

        public StoreMessageTemplateObject GetMessageTemplate(int messageEventId, string email)
        {
            try
            {
                return _messageTemplateManager.GetMessageTemplate(messageEventId, email);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageTemplateObject();
            }
        }

        public List<StoreMessageTemplateObject> GetMessageTemplates(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                var objList = _messageTemplateManager.GetMessageTemplates(itemsPerPage, pageNumber, out countG);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreMessageTemplateObject>();
			    }
                
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                countG = 0;
                return new List<StoreMessageTemplateObject>();
            }
        }

        public List<StoreMessageTemplateObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _messageTemplateManager.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<StoreMessageTemplateObject>();
                }

                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<StoreMessageTemplateObject>();
            }
        }
	}

}
