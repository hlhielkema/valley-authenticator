using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ValleyAuthenticator.Utils
{
    public class MergeObservableCollection
    {
        public static bool MatchElements<T>(ObservableCollection<T> collection, List<T> updated)
        {
            if (collection.Count == updated.Count)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (!collection[i].Equals(updated[i]))
                        return false;
                }
                return true;
            }            
            return false;
        }

        public static void Replace<T>(ObservableCollection<T> collection, List<T> updated)
        {
            if (MatchElements(collection, updated))
                return;

            for (int i = 0; i < updated.Count; i++)
            {
                if (i >= collection.Count)
                    collection.Add(updated[i]);
                else if (collection[i].Equals(updated[i]))
                    continue;
                else                
                    collection.Insert(i, updated[i]);                                    
            }

            while (collection.Count > updated.Count)
                collection.RemoveAt(collection.Count - 1);

            // Fallback
            if (!MatchElements(collection, updated))
            {
                collection.Clear();
                foreach (T item in updated)
                    collection.Add(item);
            }
        }
    }
}
