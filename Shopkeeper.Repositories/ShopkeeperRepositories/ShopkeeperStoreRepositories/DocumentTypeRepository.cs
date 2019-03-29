using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.DataObjects.DataObjects.Store;
using Shopkeeper.Infrastructures.ShopkeeperInfrastructures;
using Shopkeeper.Repositories.Utilities;
using ShopkeeperStore.EF.Models.Store;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class DocumentTypeRepository
    {
        private readonly IShopkeeperRepository<DocumentType> _repository;
       private readonly UnitOfWork _uoWork;

       public DocumentTypeRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ShopKeeperStoreEntities"].ConnectionString;
            var storeSetting = new SessionHelpers().GetStoreInfo();
            if (storeSetting != null && storeSetting.StoreId > 0)
            {
                connectionString = storeSetting.EntityConnectionString;
            }
            var shopkeeperStoreContext = new ShopKeeperStoreEntities(connectionString);
           _uoWork = new UnitOfWork(shopkeeperStoreContext);
           _repository = new ShopkeeperRepository<DocumentType>(_uoWork);
		}
       
        public long AddDocumentType(DocumentTypeObject documentType)
        {
            try
            {
                if (documentType == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.TypeName.Trim().ToLower() == documentType.TypeName.Trim().ToLower() && (m.DocumentTypeId != documentType.DocumentTypeId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var documentTypeEntity = ModelCrossMapper.Map<DocumentTypeObject, DocumentType>(documentType);
                if (documentTypeEntity == null || string.IsNullOrEmpty(documentTypeEntity.TypeName))
                {
                    return -2;
                }
                var returnStatus = _repository.Add(documentTypeEntity);
                _uoWork.SaveChanges();
                return returnStatus.DocumentTypeId;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int UpdateDocumentType(DocumentTypeObject documentType)
        {
            try
            {
                if (documentType == null)
                {
                    return -2;
                }
                var duplicates = _repository.Count(m => m.TypeName.Trim().ToLower() == documentType.TypeName.Trim().ToLower() && (m.DocumentTypeId != documentType.DocumentTypeId));
                if (duplicates > 0)
                {
                    return -3;
                }
                var documentTypeEntity = ModelCrossMapper.Map<DocumentTypeObject, DocumentType>(documentType);
                if (documentTypeEntity == null || documentTypeEntity.DocumentTypeId < 1)
                {
                    return -2;
                }
                _repository.Update(documentTypeEntity);
                _uoWork.SaveChanges();
                return 5;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return -2;
            }
        }

        public bool DeleteDocumentType(long documentTypeId)
        {
            try
            {
                var returnStatus = _repository.Remove(documentTypeId);
                _uoWork.SaveChanges();
                return returnStatus.DocumentTypeId > 0;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return false;
            }
        }

        public DocumentTypeObject GetDocumentType(long documentTypeId)
        {
            try
            {
                var myItem = _repository.GetById(documentTypeId);
                if (myItem == null || myItem.DocumentTypeId < 1)
                {
                    return new DocumentTypeObject();
                }
                var documentTypeObject = ModelCrossMapper.Map<DocumentType, DocumentTypeObject>(myItem);
                if (documentTypeObject == null || documentTypeObject.DocumentTypeId < 1)
                {
                    return new DocumentTypeObject();
                }
                return documentTypeObject;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new DocumentTypeObject();
            }
        }

        public List<DocumentTypeObject> GetDocumentTypeObjects(int? itemsPerPage, int? pageNumber)
        {
            try
            {
                List<DocumentType> documentTypeEntityList;
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;
                    documentTypeEntityList = _repository.GetWithPaging(m => m.DocumentTypeId, tpageNumber, tsize).ToList();
                }

                else
                {
                    documentTypeEntityList = _repository.GetAll().ToList();
                }

                if (!documentTypeEntityList.Any())
                {
                    return new List<DocumentTypeObject>();
                }
                var documentTypeObjList = new List<DocumentTypeObject>();
                documentTypeEntityList.ForEach(m =>
                {
                    var documentTypeObject = ModelCrossMapper.Map<DocumentType, DocumentTypeObject>(m);
                    if (documentTypeObject != null && documentTypeObject.DocumentTypeId > 0)
                    {
                        documentTypeObjList.Add(documentTypeObject);
                    }
                });

                return documentTypeObjList;
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
               var documentTypeEntityList = _repository.GetAll(m => m.TypeName.ToLower().Contains(searchCriteria.ToLower())).ToList();

                if (!documentTypeEntityList.Any())
                {
                    return new List<DocumentTypeObject>();
                }
                var documentTypeObjList = new List<DocumentTypeObject>();
                documentTypeEntityList.ForEach(m =>
                {
                    var documentTypeObject = ModelCrossMapper.Map<DocumentType, DocumentTypeObject>(m);
                    if (documentTypeObject != null && documentTypeObject.DocumentTypeId > 0)
                    {
                        documentTypeObjList.Add(documentTypeObject);
                    }
                });
                return documentTypeObjList;
            }

            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<DocumentTypeObject>();
            }
        }

        public int GetObjectCount()
        {
            try
            {
                return _repository.Count();
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public int GetObjectCount(Expression<Func<DocumentType, bool>> predicate)
        {
            try
            {
                return _repository.Count(predicate);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }

        public List<DocumentTypeObject> GetDocumentTypes()
        {
            try
            {
                var documentTypeEntityList = _repository.GetAll().ToList();
                if (!documentTypeEntityList.Any())
                {
                    return new List<DocumentTypeObject>();
                }
                var documentTypeObjList = new List<DocumentTypeObject>();
                documentTypeEntityList.ForEach(m =>
                {
                    var documentTypeObject = ModelCrossMapper.Map<DocumentType, DocumentTypeObject>(m);
                    if (documentTypeObject != null && documentTypeObject.DocumentTypeId > 0)
                    {
                        documentTypeObjList.Add(documentTypeObject);
                    }
                });
                return documentTypeObjList;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
       
    }
}
