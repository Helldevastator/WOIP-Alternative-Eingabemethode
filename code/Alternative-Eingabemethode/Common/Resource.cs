using System;

namespace Common
{
    [Serializable]
    public struct Resource
    {
        private static Object nextIdLock = new Object();
        private static int nextId = 0;

         public int ResourceId { get; private set;}
         public int ResourceType { get; private set; } 

         public Resource(int resourceType): this()
         {
             lock (nextIdLock)
             {
                 this.ResourceId = nextId;
                 nextId++;
             }

             this.ResourceType = resourceType;
         }
    }
}
