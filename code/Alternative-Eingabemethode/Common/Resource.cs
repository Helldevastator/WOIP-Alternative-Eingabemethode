using System;

namespace Common
{
    [Serializable]
    public class Resource
    {
         public int ResourceId { get; private set;}
         public int ResourceType { get; private set; } 

         public Resource(int resourceType)
         {
             lock (nextIdLock)
             {
                 this.ResourceId = nextId;
                 nextId++;
             }

             this.ResourceType = resourceType;
         }

         public Resource(int resourceId, int resourceType)
         {
             this.ResourceId = resourceId;
             this.ResourceType = resourceType;
         }
    }
}
