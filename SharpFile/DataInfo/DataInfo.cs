using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SharpFile
{
    public abstract class DataInfo
    {
        protected string name;
        protected long size;
        protected DateTime lastWriteTime;
        protected Image image;

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
