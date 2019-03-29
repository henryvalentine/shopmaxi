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
    public class StoreMessageRepository
    {
        public long AddMessage(StoreMessageObject message) 
        {
            try
            {
                if (message == null)
                {
                    return -2;
                }

                var messageEntity = ModelCrossMapper.Map<StoreMessageObject, StoreMessage>(message);

                if (messageEntity == null || messageEntity.Id < 1)
                {
                    return -2;
                }

                using (var db = new ShopKeeperStoreEntities())
                {
                    var returnStatus = db.StoreMessages.Add(messageEntity);
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


        public long AddMessagePerm(StoreMessageObject message)
        {
            try
            {
                if (message == null)
                {
                    return -2;
                }

                var messageEntity = ModelCrossMapper.Map<StoreMessageObject, StoreMessage>(message);

                if (messageEntity == null)
                {
                    return -2;
                }

                using (var db = new ShopKeeperStoreEntities())
                {
                    var returnStatus = db.StoreMessages.Add(messageEntity);
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


        public long UpdateMessage(StoreMessageObject message)
        {
            try
            {
                if (message == null)
                {
                    return -2;
                }

                using (var db = new ShopKeeperStoreEntities())
                {
                    var msgs = db.StoreMessages.Where(k => k.Id == message.Id).ToList();
                    if (!msgs.Any())
                    {
                        return -2;
                    }
                    var msg = msgs[0];
                    msg.MessageTemplateId = message.MessageTemplateId;
                    msg.Status = message.Status;
                    msg.DateSent = msg.DateSent;
                    msg.MessageBody = message.MessageBody;
                    db.Entry(msg).State = EntityState.Modified;
                    db.SaveChanges();
                    return msg.Id;
                }
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
                using (var db = new ShopKeeperStoreEntities())
                {
                    var countries = db.StoreMessages.ToList();
                    if (!countries.Any())
                    {
                        return new List<StoreMessageObject>();
                    }
                    var objList = new List<StoreMessageObject>();
                    countries.ForEach(app =>
                    {
                        var messageObject = ModelCrossMapper.Map<StoreMessage, StoreMessageObject>(app);
                        if (messageObject != null && messageObject.Id > 0)
                        {
                            objList.Add(messageObject);
                        }
                    });

                    return !objList.Any() ? new List<StoreMessageObject>() : objList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
        
        public List<StoreMessageObject> GetMessages(int? itemsPerPage, int? pageNumber, out int countG)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = new ShopKeeperStoreEntities())
                    {
                        var messages = (from msg in db.StoreMessages.OrderByDescending(g => g.DateSent).Include("StoreMessageTemplate")
                             .Skip(tpageNumber).Take(tsize) 
                             join usr in db.UserProfiles.Include("AspNetUsers") on msg.Id equals usr.Id
                             select new StoreMessageObject
                             {
                                 EventTypeId = msg.StoreMessageTemplate.EventTypeId,
                                 UserId = msg.UserId,
                                 Id = msg.Id,
                                 MessageTemplateId = msg.MessageTemplateId,
                                 Status = msg.Status,
                                 Receipient = usr.LastName + " " + usr.OtherNames,
                                 Subject = msg.StoreMessageTemplate.Subject,
                                 MessageContent = msg.StoreMessageTemplate.MessageContent,
                                 Footer = msg.StoreMessageTemplate.Footer

                             }).ToList();
                        
                        if (messages.Any())
                        {
                            messages.ForEach(app =>
                            {
                                var msgEvent = Enum.GetName(typeof(MessageEventEnum), app.EventTypeId);
                                if (msgEvent != null)
                                {
                                    app.EventTypeName = msgEvent.Replace("_", " ");

                                }
                                app.StatusStr = Enum.GetName(typeof(MessageStatus), app.Status).Replace("_", " ");

                                app.DateSentStr = app.DateSent.ToString("dd/MM/yyyy"); 
                            });
                            countG = db.StoreMessages.Count();
                            return messages;
                        }
                    }

                }
                countG = 0;
                return new List<StoreMessageObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> GetMyMessages(int? itemsPerPage, int? pageNumber, out int countG, long personId)
        {
            try
            {
                if ((itemsPerPage != null && itemsPerPage > 0) && (pageNumber != null && pageNumber >= 0))
                {
                    var tpageNumber = (int)pageNumber;
                    var tsize = (int)itemsPerPage;

                    using (var db = new ShopKeeperStoreEntities())
                    {
                        var messages = (from msg in db.StoreMessages.Where(m => m.Id == personId).OrderByDescending(g => g.Id).Include("StoreMessageTemplate")
                             .Skip(tpageNumber).Take(tsize)
                                       
                                        select new StoreMessageObject
                                        {
                                            EventTypeId = msg.StoreMessageTemplate.EventTypeId,
                                            UserId = msg.UserId,
                                            Id = msg.Id,
                                            MessageTemplateId = msg.MessageTemplateId,
                                            Status = msg.Status,
                                            Subject = msg.StoreMessageTemplate.Subject,
                                            MessageContent = msg.StoreMessageTemplate.MessageContent,
                                            Footer = msg.StoreMessageTemplate.Footer,
                                            DateSent = msg.DateSent
                                        }).ToList();

                        if (messages.Any())
                        {
                            messages.ForEach(app =>
                            {
                                var msgEvent = Enum.GetName(typeof(MessageEventEnum), app.EventTypeId);
                                if (msgEvent != null)
                                {
                                    app.EventTypeName = msgEvent.Replace("_", " ");

                                }
                                app.StatusStr = Enum.GetName(typeof(MessageStatus), app.Status).Replace("_", " ");

                                app.DateSentStr = app.DateSent.ToString("dd/MM/yyyy"); 
                            });
                            countG = db.StoreMessages.Count(m => m.Id == personId);
                            return messages;
                        }
                    }

                }
                countG = 0;
                return new List<StoreMessageObject>();
            }
            catch (Exception ex)
            {
                countG = 0;
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> GetMyLatestMessages(long personId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var messages = (from msg in db.StoreMessages.Where(m => m.Id == personId).OrderByDescending(g => g.Id).Include("StoreMessageTemplate").Take(5)
                                    select new StoreMessageObject
                                    {
                                        EventTypeId = msg.StoreMessageTemplate.EventTypeId,
                                        UserId = msg.UserId,
                                        Id = msg.Id,
                                        MessageTemplateId = msg.MessageTemplateId,
                                        Status = msg.Status,
                                        Subject = msg.StoreMessageTemplate.Subject,
                                        MessageContent = msg.StoreMessageTemplate.MessageContent,
                                        Footer = msg.StoreMessageTemplate.Footer,
                                        DateSent = msg.DateSent
                                    }).ToList();

                    if (messages.Any())
                    {
                        messages.ForEach(app =>
                        {
                            var msgEvent = Enum.GetName(typeof(MessageEventEnum), app.EventTypeId);
                            if (msgEvent != null)
                            {
                                app.EventTypeName = msgEvent.Replace("_", " ");

                            }
                            app.StatusStr = Enum.GetName(typeof(MessageStatus), app.Status).Replace("_", " ");

                            app.DateSentStr = app.DateSent.ToString("dd/MM/yyyy");
                        });
                        return messages;
                    }
                    return new List<StoreMessageObject>();
                }
            }
            catch (Exception ex)
            {
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> GetMyMessages(long personId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var messages = (from msg in db.StoreMessages.Where(m => m.Id == personId).OrderByDescending(g => g.DateSent).Include("StoreMessageTemplate")
                                    join usr in db.UserProfiles.Include("AspNetUsers") on msg.Id equals usr.Id

                                    select new StoreMessageObject
                                    {
                                        EventTypeId = msg.StoreMessageTemplate.EventTypeId,
                                        UserId = msg.UserId,
                                        Id = msg.Id,
                                        MessageTemplateId = msg.MessageTemplateId,
                                        Status = msg.Status,
                                        Subject = msg.StoreMessageTemplate.Subject,
                                        MessageContent = msg.MessageBody,
                                        Footer = msg.StoreMessageTemplate.Footer, 
                                        DateSent = msg.DateSent

                                    }).ToList();

                    if (!messages.Any())
                    {
                        return new List<StoreMessageObject>();
                    }
                    messages.ForEach(app =>
                    {
                        var msgEvent = Enum.GetName(typeof(MessageEventEnum), app.EventTypeId);
                        if (msgEvent != null)
                        {
                            app.EventTypeName = msgEvent.Replace("_", " ");

                        }
                        app.StatusStr = Enum.GetName(typeof (MessageStatus), app.Status).Replace("_", " ");

                        app.DateSentStr = app.DateSent.ToString("dd/MM/yyyy"); 
                    });
                    return messages;
                }
            }
            catch (Exception ex)
            {
                return new List<StoreMessageObject>();
            }
        }

        public List<StoreMessageObject> SearchMessages(string searchCriteria, long personId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var messages = (from msg in db.StoreMessages.Where(m => m.Id == personId && m.StoreMessageTemplate.Subject.Contains(searchCriteria)).OrderByDescending(g => g.DateSent).Include("StoreMessageTemplate")

                                    select new StoreMessageObject
                                    {
                                        EventTypeId = msg.StoreMessageTemplate.EventTypeId,
                                        Id = msg.Id,
                                        MessageTemplateId = msg.MessageTemplateId,
                                        Status = msg.Status,
                                        Subject = msg.StoreMessageTemplate.Subject,
                                        MessageContent = msg.MessageBody,
                                        Footer = msg.StoreMessageTemplate.Footer,
                                        DateSent = msg.DateSent

                                    }).ToList();

                    if (!messages.Any())
                    {
                        return new List<StoreMessageObject>();
                    }
                    messages.ForEach(app =>
                    {
                        var msgEvent = Enum.GetName(typeof(MessageEventEnum), app.EventTypeId);
                        if (msgEvent != null)
                        {
                            app.EventTypeName = msgEvent.Replace("_", " ");

                        }
                        app.StatusStr = Enum.GetName(typeof(MessageStatus), app.Status).Replace("_", " ");

                        app.DateSentStr = app.DateSent.ToString("dd/MM/yyyy");
                    });
                    return messages;
                }
            }
            catch (Exception ex)
            {
                return new List<StoreMessageObject>();
            }
        }

        public StoreMessageObject GetMessage(long messageId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var messages = (from msg in db.StoreMessages.Where(o => o.Id == messageId).OrderByDescending(g => g.DateSent).Include("StoreMessageTemplate")
                                    join usr in db.UserProfiles.Include("AspNetUsers") on msg.Id equals usr.Id
                                    select new StoreMessageObject
                                    {
                                        EventTypeId = msg.StoreMessageTemplate.EventTypeId,
                                        UserId = msg.Id,
                                        Id = msg.Id,
                                        MessageTemplateId = msg.MessageTemplateId,
                                        Status = msg.Status,
                                        Subject = msg.StoreMessageTemplate.Subject,
                                        MessageContent = msg.MessageBody,
                                        Footer = msg.StoreMessageTemplate.Footer

                                    }).ToList();

                    if (!messages.Any())
                    {
                        return new StoreMessageObject();
                    }

                    var app = messages[0];
                    var msgEvent = Enum.GetName(typeof(MessageEventEnum), app.EventTypeId);
                    if (msgEvent != null)
                    {
                        app.EventTypeName = msgEvent.Replace("_", " ");

                    }
                    var status = Enum.GetName(typeof(MessageStatus), app.Status);
                    if (status != null)
                    {
                        app.StatusStr = status.Replace("_", " ");
                    }
                    return app;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new StoreMessageObject();
            }
        }

        public List<StoreMessageObject> Search(string searchCriteria)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var messages = (from msgt in db.StoreMessageTemplates.Where(u => u.Subject.ToLower().Contains(searchCriteria.ToLower()))
                                    join msg in db.StoreMessages.OrderByDescending(g => g.DateSent) on msgt.Id equals msg.MessageTemplateId
                                    join usr in db.UserProfiles.Include("AspNetUsers") on msg.Id equals usr.Id
                                    select new StoreMessageObject
                                    {
                                        EventTypeId = msgt.EventTypeId,
                                        UserId = msg.Id,
                                        Id = msg.Id,
                                        MessageTemplateId = msgt.Id,
                                        Status = msg.Status,
                                        Receipient = usr.LastName + " " + usr.OtherNames,
                                        Subject = msgt.Subject,
                                        MessageContent = msgt.MessageContent,
                                        Footer = msgt.Footer

                                    }).ToList();

                    if (!messages.Any())
                    {
                        return new List<StoreMessageObject>();
                    }

                    messages.ForEach(app =>
                    {
                        var msgEvent = Enum.GetName(typeof(MessageEventEnum), app.EventTypeId);
                        if (msgEvent != null)
                        {
                            app.EventTypeName = msgEvent.Replace("_", " ");

                        }
                        var status = Enum.GetName(typeof(MessageStatus), app.Status);
                        if (status != null)
                        {
                            app.EventTypeName = status.Replace("_", " ");
                        }
                    });
                    return messages;
                }
            }
            catch (Exception ex)
            {
                return new List<StoreMessageObject>();
            }
        }
        public long DeleteMessage(long messageId)
        {
            try
            {
                using (var db = new ShopKeeperStoreEntities())
                {
                    var myItems =
                        db.StoreMessages.Where(m => m.Id == messageId).ToList();
                    if (!myItems.Any())
                    {
                        return 0;
                    }

                    var item = myItems[0];
                    db.StoreMessages.Remove(item);
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
