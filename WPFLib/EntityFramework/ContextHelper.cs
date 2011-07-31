using System.Linq;
using System.Data;
using System.Data.Objects;
using System.Collections.Generic;

namespace System
{
    public static class ContextHelper
    {
        public static IEnumerable<T> Local<T>(this ObjectSet<T> objectSet) where T : class
        {
            return from stateEntry in objectSet.Context.ObjectStateManager
                                               .GetObjectStateEntries(EntityState.Added |
                                                                      EntityState.Modified |
                                                                      EntityState.Unchanged)
                   where stateEntry.Entity != null && stateEntry.EntitySet == objectSet.EntitySet
                   select stateEntry.Entity as T;
        }

        public static void AttachAsAdded<T>(this ObjectSet<T> contextset, T view ) where T : class
        {
            contextset.Attach(view);
            contextset.Context.ObjectStateManager.ChangeObjectState(view, EntityState.Added);
        }

        public static void AttachAsModified<T>( this ObjectSet<T> contextset,T view) where T : class
        {
            contextset.Attach(view);
            contextset.Context.ObjectStateManager.ChangeObjectState(view, EntityState.Modified);
        }
    }
}
