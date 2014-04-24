using System;

namespace Common
{
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

         public Resource(int resourceId, int resourceType) : this()
         {
             this.ResourceId = resourceId;
             this.ResourceType = resourceType;
         }
    }
}
