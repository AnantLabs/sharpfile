using System.Collections.Generic;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
    public abstract class ChildResourceRetriever : IChildResourceRetriever {
        private List<ColumnInfo> columnInfos;
        private string name;
        private IView view;
        private List<string> customMethodArguments;

        public event SharpFile.Infrastructure.ChildResourceRetriever.GetCompleteDelegate GetComplete;
        public event SharpFile.Infrastructure.ChildResourceRetriever.CustomMethodDelegate CustomMethod;
        public event SharpFile.Infrastructure.ChildResourceRetriever.CustomMethodWithArgumentsDelegate CustomMethodWithArguments;

        public void OnGetComplete() {
            if (GetComplete != null) {
                GetComplete();
            }
        }

        public bool OnCustomMethod(IResource resource) {
            if (CustomMethod != null) {
                return CustomMethod(resource);
            }

            return false;
        }

        public bool OnCustomMethodWithArguments(IResource resource, List<string> arguments) {
            if (CustomMethodWithArguments != null) {
                return CustomMethodWithArguments(resource, arguments);
            }

            return false;
        }

        public abstract void Execute(IView view, IResource resource);

        public abstract IChildResourceRetriever Clone();

        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;
            }
        }

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public IView View {
            get {
                return view;
            }
            set {
                view = value;
            }
        }

        public List<string> CustomMethodArguments {
            get {
                return customMethodArguments;
            }
            set {
                customMethodArguments = value;
            }
        }
    }
}