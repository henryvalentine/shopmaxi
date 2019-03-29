using System;
using System.Collections.Generic;
using System.Linq;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Master;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperMasterRepositories;

namespace ShopkeeperServices.ShopkeeperServices.ShopkeeperMasterServices
{
	public class MessageServices
	{
        private readonly MessageRepository _messageRepository;
        public MessageServices()
		{
            _messageRepository = new MessageRepository();
		}

        public long AddMessage(MessageObject message)
		{
			try
			{
                return _messageRepository.AddMessage(message);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public long AddMessagePerm(MessageObject message)
        {
            try
            {
                return _messageRepository.AddMessagePerm(message);
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
                return _messageRepository.DeleteMessage(messageId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public long UpdateMessage(MessageObject message)
        {
            try
            {
                return _messageRepository.UpdateMessage(message);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
        
        public List<MessageObject> GetMessages()
		{
			try
			{
                var objList = _messageRepository.GetMessages();
                if (objList == null || !objList.Any())
			    {
                    return new List<MessageObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<MessageObject>();
			}
		}

        public MessageObject GetMessage(long messageId)
        {
            try
            {
                return _messageRepository.GetMessage(messageId);
                
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new MessageObject();
            }
        }

        public List<MessageObject> GetMessages(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                var objList = _messageRepository.GetMessages(itemsPerPage, pageNumber, out countG);
                if (objList == null || !objList.Any())
                {
                    return new List<MessageObject>();
			    }
                
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                countG = 0;
                return new List<MessageObject>();
            }
        }

        public List<MessageObject> GetMyMessages(int? itemsPerPage, int? pageNumber, out int countG, long userId)
        {
            try
            {
                return _messageRepository.GetMyMessages(itemsPerPage, pageNumber, out countG, userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                countG = 0;
                return new List<MessageObject>();
            }
        }

        public List<MessageObject> GetMyLatestMessages(long userId)
        {
            try
            {
                return _messageRepository.GetMyLatestMessages(userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<MessageObject>();
            }
        }

        public List<MessageObject> SearchMessages(string searchCriteria, long userId)
        {
            try
            {
                return _messageRepository.SearchMessages(searchCriteria, userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<MessageObject>();
            }
        }

        public List<MessageObject> GetMyMessages(long userId)
        {
            try
            {
                return _messageRepository.GetMyMessages(userId);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<MessageObject>();
            }
        }

        public List<MessageObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _messageRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<MessageObject>();
                }

                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<MessageObject>();
            }
        }
	}

}
