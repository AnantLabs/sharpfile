using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SharpFile
{
    public abstract class DataInfo
    {
		protected string displayName;
        protected string name;
        protected long size;
        protected DateTime lastWriteTime;
        protected Image image;

		public string DisplayName {
			get {
				if (string.IsNullOrEmpty(displayName)) {
					return name;
				} else {
					return displayName;
				}
			}
		}

        public string Name
        {
            get
            {
                return name;
            }
        }

        public long Size
        {
            get
            {
                return size;
            }
        }

        public DateTime LastWriteTime
        {
            get
            {
                return lastWriteTime;
            }
        }
    }
}
