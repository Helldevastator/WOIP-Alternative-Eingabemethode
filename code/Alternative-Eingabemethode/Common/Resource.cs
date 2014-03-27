using System;

namespace Common
{
    [Serializable]
    public sealed class Resource
    {
        private static Object nextIdLock = new Object();
        private static int nextId = 0;

         public readonly int ResourceId {public get; private set;}
         public readonly int ResourceType { public get; private set; } 
         public readonly string ResourceName {public get; private set;}

         public Resource(string resourceName, int resourceType)
         {

             lock (nextIdLock)
             {
                 this.ResourceId = nextId;
                 nextId++;
             }

             this.ResourceType = resourceType;
             this.ResourceName = resourceName;
         }
    }
}
