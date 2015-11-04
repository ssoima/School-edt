using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NextLAP.IP1.Common;

namespace NextLAP.IP1.PlanningWebAPI.Helper
{
    public static class LinkedEntityHelper
    {
        public static bool AddLinkedEntity<T>(IQueryable<T> q, T entity, ILinkedEntity template)
            where T : class, ILinkedEntity<T>
        {
            if (entity.Id == template.PredecessorId) return false;
            if (template.PredecessorId == null)
            {
                entity.PredecessorId = null;
                // the model to insert is at first place
                var entityToMove = q.FirstOrDefault(x => x.PredecessorId == null);
                if (entityToMove != null)
                {
                    entityToMove.Predecessor = entity;
                    return true;
                }
            }
            else
            {
                var entitiesInvolved =
                    q.Where(x => x.Id == template.PredecessorId || x.PredecessorId == template.PredecessorId).ToList();
                if (entitiesInvolved.Count == 0)
                    throw new InvalidOperationException("There is no entity in movement involved.");
                // now we have up to max two entities
                var entityBecomingPredecessor = entitiesInvolved.FirstOrDefault(x => x.Id == template.PredecessorId);
                if (entityBecomingPredecessor == null)
                    throw new InvalidOperationException("There is no entity with ID:" + template.PredecessorId +
                                                        ". Please provide the PredecessorId, which will preceed the entity to create.");

                entity.Predecessor = entityBecomingPredecessor;
                // check if there is another entity which needs to 
                var entityGettingNewPredecessor =
                    entitiesInvolved.FirstOrDefault(x => x.PredecessorId == template.PredecessorId);
                if (entityGettingNewPredecessor != null)
                    entityGettingNewPredecessor.Predecessor = entity;
                return true;
            }
            return false;
        }

        public static bool RemoveLinkedEntity<T>(IQueryable<T> q, T entity)
            where T : class, ILinkedEntity<T>
        {
            if (entity.PredecessorId == null)
            {
                // this entity is the first in a row
                var entityToMove = q.FirstOrDefault(x => x.PredecessorId == entity.Id);
                if (entityToMove != null)
                {
                    entityToMove.Predecessor = null;
                    return true;
                }
            }
            else
            {
                var entitiesInvolved =
                    q.Where(x => x.Id == entity.PredecessorId || x.PredecessorId == entity.Id).ToList();
                if (entitiesInvolved.Count == 0) throw new InvalidOperationException("There is no entity in movement involved.");
                // now we have up to max two entities
                var predecessor = entitiesInvolved.FirstOrDefault(x => x.Id == entity.PredecessorId);
                var successor = entitiesInvolved.FirstOrDefault(x => x.PredecessorId == entity.Id);
                if (successor != null)
                {
                    successor.Predecessor = predecessor;
                    return true;
                }
            }
            return false;
        }

        public static bool MoveLinkedEntity<T>(IQueryable<T> q, T entity, ILinkedEntity template)
            where T : class, ILinkedEntity<T>
        {
            if (entity.Id == template.PredecessorId) return false;
            var remove = RemoveLinkedEntity(q, entity);
            var add = AddLinkedEntity(q, entity, template);

            return remove && add;

            #region Old

            //var changes = false;
            //// first restore original link
            //if (entity.PredecessorId == null)
            //{
            //    // this entity is the first in a row
            //    var entityToMove = q.FirstOrDefault(x => x.PredecessorId == entity.Id);
            //    if (entityToMove != null)
            //    {
            //        entityToMove.Predecessor = null;
            //        changes = true;
            //    }
            //}
            //else
            //{
            //    var entitiesInvolved =
            //        q.Where(x => x.Id == entity.PredecessorId || x.PredecessorId == entity.Id).ToList();
            //    if (entitiesInvolved.Count == 0) throw new InvalidOperationException("There is no entity in movement involved.");
            //    // now we have up to max two entities
            //    var predecessor = entitiesInvolved.FirstOrDefault(x => x.Id == entity.PredecessorId);
            //    var successor = entitiesInvolved.FirstOrDefault(x => x.PredecessorId == entity.Id);
            //    if (successor != null)
            //    {
            //        successor.Predecessor = predecessor;
            //        changes = true;
            //    }
            //}
            //// now move to the correct place
            //if (template.PredecessorId == null)
            //{
            //    // the model to insert is at first place
            //    var entityToMove = q.FirstOrDefault(x => x.PredecessorId == null);
            //    if (entityToMove != null)
            //    {
            //        entityToMove.Predecessor = entity;
            //        changes = true;
            //    }
            //}
            //else
            //{
            //    var stationsInvolved = q.Where(x => x.Id == template.PredecessorId || x.PredecessorId == template.PredecessorId)
            //            .ToList();
            //    if (stationsInvolved.Count == 0) throw new InvalidOperationException("There is no entity in movement involved.");
            //    // now we have up to max two entities
            //    var stationBecomingPredecessor = stationsInvolved.FirstOrDefault(x => x.Id == template.PredecessorId);
            //    if (stationBecomingPredecessor == null)
            //        throw new InvalidOperationException("There is no entity with ID:" + template.PredecessorId +
            //                                            ". Please provide the PredecessorId, which will preceed the entity to create.");
            //    entity.Predecessor = stationBecomingPredecessor;
            //    // check if there is another entity which needs to 
            //    var stationGettingNewPredecessor =
            //        stationsInvolved.FirstOrDefault(x => x.PredecessorId == template.PredecessorId);
            //    if (stationGettingNewPredecessor != null)
            //        stationGettingNewPredecessor.Predecessor = entity;
            //    changes = true;
            //}
            //return changes;

            #endregion
        }
    }
}