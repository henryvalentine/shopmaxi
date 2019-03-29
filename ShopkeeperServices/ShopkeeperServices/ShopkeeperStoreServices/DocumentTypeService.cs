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
	public class DocumentTypeServices
	{
		private readonly DocumentTypeRepository  _documentTypeAccountRepository;
        public DocumentTypeServices()
		{
            _documentTypeAccountRepository = new DocumentTypeRepository();
		}

        public long AddDocumentType(DocumentTypeObject documentTypeAccount)
		{
			try
			{
                return _documentTypeAccountRepository.AddDocumentType(documentTypeAccount);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return 0;
			}
		}

        public int GetObjectCount()
        {
            try
            {
                return _documentTypeAccountRepository.GetObjectCount();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateDocumentType(DocumentTypeObject documentTypeAccount)
		{
			try
			{
                return _documentTypeAccountRepository.UpdateDocumentType(documentTypeAccount);
            }
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return -2;
			}
		}

        public bool DeleteDocumentType(long documentTypeAccountId)
		{
			try
			{
                return _documentTypeAccountRepository.DeleteDocumentType(documentTypeAccountId);
				}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
				return false;
			}
		}

        public DocumentTypeObject GetDocumentType(long documentTypeAccountId)
		{
			try
			{
                return _documentTypeAccountRepository.GetDocumentType(documentTypeAccountId);
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new DocumentTypeObject();
			}
		}

        public List<DocumentTypeObject> GetDocumentTypes()
		{
			try
			{
                var objList = _documentTypeAccountRepository.GetDocumentTypes();
                if (objList == null || !objList.Any())
			    {
                    return new List<DocumentTypeObject>();
			    }
				return objList;
			}
			catch (Exception ex)
			{
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DocumentTypeObject>();
			}
		}

        public List<DocumentTypeObject> GetDocumentTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                var objList = _documentTypeAccountRepository.GetDocumentTypeObjects(itemsPerPage, pageNumber);
                if (objList == null || !objList.Any())
                {
                    return new List<DocumentTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DocumentTypeObject>();
            }
        }

        public List<DocumentTypeObject> Search(string searchCriteria)
        {
            try
            {
                var objList = _documentTypeAccountRepository.Search(searchCriteria);
                if (objList == null || !objList.Any())
                {
                    return new List<DocumentTypeObject>();
                }
                return objList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DocumentTypeObject>();
            }
        }
	}

}
