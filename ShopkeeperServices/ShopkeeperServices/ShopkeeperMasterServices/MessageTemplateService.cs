using System;
using System.Collections.Generic;
using System.Linq;
using ImportPermitPortal.DataObjects;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class MessageTemplateServices
	{
        private readonly MessageTemplateRepository _messageTemplateRepository;
        public MessageTemplateServices()
		{
            _messageTemplateRepository = new MessageTemplateRepository();
		}

        public long AddMessageTemplate(MessageTemplateObject messageTemplate)
		{
			try
			{
                return _messageTemplateRepository.AddMessageTemplate(messageTemplate);
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
                return _messageTemplateRepository.DeleteMessageTemplate(messageTemplateId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long UpdateMessageTemplate(MessageTemplateObject messageTemplate)
        {
            try
            {
                return _messageTemplateRepository.UpdateMessageTemplate(messageTemplate);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public List<MessageTemplateObject> GetMessageTemplates()
		{
			try
			{
                var objList = _messageTemplateRepository.GetMessageTemplates();
                if (objList == null || !objList.Any())
			    {
                    return new List<MessageTemplateObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<MessageTemplateObject>();
			}
		}

        public MessageTemplateObject GetMessageTemplate(long messageTemplateId)
        {
            try
            {
                return _messageTemplateRepository.GetMessageTemplate(messageTemplateId);
                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new MessageTemplateObject();
            }
        }

        public MessageTemplateObject GetMessageTemp(int messageEventId)
        {
            try
            {
                return _messageTemplateRepository.GetMessageTemp(messageEventId);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new MessageTemplateObject();
            }
        }

        public MessageTemplateObject GetMessageTemp(int messageEventId, string email)
        {
            try
            {
                return _messageTemplateRepository.GetMessageTemp(messageEventId, email);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new MessageTemplateObject();
            }
        }

        public MessageTemplateObject GetMessageTemplate(int messageEventId)
        {
            try
            {
                return _messageTemplateRepository.GetMessageTemplate(messageEventId);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new MessageTemplateObject();
            }
        }

        public MessageTemplateObject GetMessageTemplate(int messageEventId, string email)
        {
            try
            {
                return _messageTemplateRepository.GetMessageTemplate(messageEventId, email);

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new MessageTemplateObject();
            }
        }

        public List<MessageTemplateObject> GetMessageTemplates(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                var objList = _messageTemplateRepository.GetMessageTemplates(itemsPerPage, pageNumber, out countG);
                if (objList == null || !objList.Any())
                {
                    return new List<MessageTemplateObject>();
			    }
                
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                countG = 0;
                return new List<MessageTemplateObject>();
            }
        }

        public List<MessageTemplateObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _messageTemplateRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<MessageTemplateObject>();
                }

                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<MessageTemplateObject>();
            }
        }
	}

}
