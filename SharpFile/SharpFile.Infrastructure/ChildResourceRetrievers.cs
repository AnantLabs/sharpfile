using System.Collections.Generic;

namespace SharpFile.Infrastructure {
    public class ChildResourceRetrievers : List<IChildResourceRetriever> {
        public ChildResourceRetrievers()
            : base() {
        }

        public ChildResourceRetrievers(int capacity)
            : base(capacity) {
        }

        public IEnumerable<IChildResourceRetriever> Filter(IResource resource) {
            foreach (IChildResourceRetriever childResourceRetriever in this) {
                if (childResourceRetriever.OnCustomMethod(resource)) {
                    yield return childResourceRetriever;
                }
            }
        }

        public ChildResourceRetrievers Clone() {
            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers(this.Count);

            int count = 0;
            foreach (IChildResourceRetriever childResourceRetriever in this) {
                childResourceRetrievers.Add(childResourceRetriever.Clone());
                childResourceRetrievers[count].CustomMethod += childResourceRetriever.OnCustomMethod;
                count++;
            }

            return childResourceRetrievers;
        }

        public static bool DefaultCustomMethod(IResource resource) {
            if (resource.Name.Equals("Programming")) {
                return false;
            }
 
            return true;
        }

        public static bool ProgrammingCustomMethod(IResource resource) {
            if (resource.Name.Equals("Programming")) {
                return true;
            }

            return false;
        }
    }
}