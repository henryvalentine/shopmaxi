using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using IShopkeeperServices.ModelMapper;
using Shopkeeper.Datacontracts.Helpers;
using Shopkeeper.DataObjects.DataObjects.Store;
using ShopkeeperStore.EF.Models.Store;

namespace Shopkeeper.Repositories.ShopkeeperRepositories.ShopkeeperStoreRepositories
{
    public class StoreMessageTemplateRepository
    {
        public long AddMessageTemplate(StoreMessageTemplateObject messageTemplate)
        {
            try
            {
                if (messageTemplate == null)
                {
                    return -2;
                }

                var messageTemplateEntity = ModelCrossMapper.Map<StoreMessageTemplateObject, StoreMessageTemplate>(messageTemplate);

                if (messageTemplateEntity == null || messageTemplateEntity.EventTypeId < 1)
                {
                    return -2;
                }

                using (var db = new ShopKeeperStoreEntities())
                {
                    var returnStatus = db.StoreMessageTemplates.Add(messageTemplateEntity);
                    db.SaveChanges();
                    return returnStatus.Id;
                }
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
                if (messageTemplate == null)
                {
                    return -2;
                }

                var messageTemplateEntity = ModelCrossMapper.Map<StoreMessageTemplateObject, StoreMessageTemplate>(messageTemplate);
                if (messageTemplateEntity == null || messageTemplateEntity.Id < 1)
                {
                    return -2;
                }

                using (var db = new ShopKeeperStoreEntities())
                {
                    db.StoreMessageTemplates.Attach(messageTemplateEntity);
                    db.Entry(messageTemplateEntity).State = EntityState.Modified;
                    db.SaveChanges();
                    return messageTemplate.Id;
                }
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
                using (var db = new ShopKeeperStoreEntities())
                {
                    var templates = db.StoreMessageTemplates.ToList();
                    if (!templates.Any())
                    {
                        return new List<StoreMessageTemplateObject>();
                    }
                    var objList = new List<StoreMessageTemplateObject>();
                    templates.ForEach(app =>
                    {
                        var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                        if (messageTemplateObject != null && messageTemplateObject.Id > 0)
                        {
                            objList.Add(messageTemplateObject);
                        }
                    });

                    return !objList.Any() ? new List<StoreMessageTemplateObject>() : objList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

      
        public List<StoreMessageTemplateObject> GetMessageTemplates(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = new ShopKeeperStoreEntities())
                    {
                        var templates =
                            db.StoreMessageTemplates
                            .OrderByDescending(m => m.Id)
                             .Skip(tpageNumber).Take(tsize)
                             .ToList();
                        if (templates.Any())
                        {
                            var newList = new List<StoreMessageTemplateObject>();
                            templates.ForEach(app =>
                            {
                                var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                                if (messageTemplateObject != null && messageTemplateObject.Id > 0)
                                {
                                    var msgEvent = Enum.GetName(typeof (MessageEventEnum), messageTemplateObject.EventTypeId);
                                    if (msgEvent != null)
                                    {
                                        messageTemplateObject.EventTypeName = msgEvent.Replace("_", " ");
                                    }

                                    newList.Add(messageTemplateObject);
                                }
                            });
                            countG = db.StoreMessageTemplates.Count();
                            return newList;
                        }
                    }

                }
                countG = 0;
                return new List<StoreMessageTemplateObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreMessageTemplateObject>();
            }
        }

        public StoreMessageTemplateObject GetMessageTemplate(long messageTemplateId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var templates =
                        db.StoreMessageTemplates.Where(m => m.Id == messageTemplateId)
                            .ToList();
                    if (!templates.Any())
                    {
                        return new StoreMessageTemplateObject();
                    }

                    var app = templates[0];
                    var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                    if (messageTemplateObject == null || messageTemplateObject.Id < 1)
                    {
                        return new StoreMessageTemplateObject();
                    }
                    var msgEvent = Enum.GetName(typeof(MessageEventEnum), messageTemplateObject.EventTypeId);
                    if (msgEvent != null)
                    {
                        messageTemplateObject.EventTypeName = msgEvent.Replace("_", " ");
                    }

                    return messageTemplateObject;
                }
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
                using (var db = new ShopKeeperStoreEntities())
                {
                    var templates = db.StoreMessageTemplates.Where(m => m.EventTypeId == messageEventId).ToList();
                    if (!templates.Any())
                    {
                        return new StoreMessageTemplateObject();
                    }
                   
                    var app = templates[0];

                    var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                    if (messageTemplateObject == null || messageTemplateObject.Id < 1)
                    {
                        return new StoreMessageTemplateObject();
                    }
                    
                    return messageTemplateObject;
                }
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
                using (var db = new ShopKeeperStoreEntities())
                {
                    var templates = db.StoreMessageTemplates.Where(m => m.EventTypeId == messageEventId).ToList();
                    if (!templates.Any())
                    {
                        return new StoreMessageTemplateObject();
                    }

                    var users = db.AspNetUsers.Where(e => e.Email == email).Include("UserProfile").ToList();
                    if (!users.Any())
                    {
                        return new StoreMessageTemplateObject();
                    }

                    var app = templates[0];

                    var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                    if (messageTemplateObject == null || messageTemplateObject.Id < 1)
                    {
                        return new StoreMessageTemplateObject();
                    }

                    return messageTemplateObject;
                }
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
                using (var db = new ShopKeeperStoreEntities())
                {
                    var templates = db.StoreMessageTemplates.Where(m => m.EventTypeId == messageEventId).ToList();
                    
                    var app = templates[0];
                    var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                    if (messageTemplateObject == null || messageTemplateObject.Id < 1)
                    {
                        return new StoreMessageTemplateObject();
                    }
                   
                    return messageTemplateObject;
                }
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
                using (var db = new ShopKeeperStoreEntities())
                {
                    var users = db.AspNetUsers.Where(a => a.Email == email).Include("UserProfile").ToList();
                    if (!users.Any())
                    {
                        return new StoreMessageTemplateObject();
                    }
                    var templates = db.StoreMessageTemplates.Where(m => m.EventTypeId == messageEventId).ToList();
                  
                    var app = templates[0];
                    var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                    if (messageTemplateObject == null || messageTemplateObject.Id < 1)
                    {
                        return new StoreMessageTemplateObject();
                    }
                    var userId = users[0].UserProfile.Id;
                    messageTemplateObject.UserId = userId;
                    return messageTemplateObject;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageTemplateObject();
            }
        }

        public List<StoreMessageTemplateObject> Search(string searchCriteria)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var templates = db.StoreMessageTemplates.Where(m => m.Subject.ToLower() == searchCriteria.ToLower().Trim()).ToList();
                    if (!templates.Any())
                    {
                        return new List<StoreMessageTemplateObject>();
                    }
                    var newList = new List<StoreMessageTemplateObject>();
                    templates.ForEach(app =>
                    {
                        var messageTemplateObject = ModelCrossMapper.Map<StoreMessageTemplate, StoreMessageTemplateObject>(app);
                        if (messageTemplateObject != null && messageTemplateObject.EventTypeId > 0)
                        {
                            var msgEvent = Enum.GetName(typeof(MessageEventEnum), messageTemplateObject.EventTypeId);
                            if (msgEvent != null)
                            {
                                messageTemplateObject.EventTypeName = msgEvent.Replace("_", " ");
                            }

                            newList.Add(messageTemplateObject);
                        }
                    });
                    return newList;
                }
            }
            catch (Exception ex)
            {
                return new List<StoreMessageTemplateObject>();
            }
        }

        public long DeleteMessageTemplate(long messageTemplateId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var myItems =
                        db.StoreMessageTemplates.Where(m => m.Id == messageTemplateId).ToList();
                    if (!myItems.Any())
                    {
                        return 0;
                    }

                    var item = myItems[0];
                    db.StoreMessageTemplates.Remove(item);
                    db.SaveChanges();
                    return 5;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return 0;
            }
        }
    }
}
