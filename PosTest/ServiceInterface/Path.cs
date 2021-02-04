using System.Text;
using ServiceInterface.Interface;

namespace ServiceInterface
{
    public  class Path: IPathBuilder
    {
        private string _pathName;
        private object _argValue;
        private IPath _parentPath;
        private IPath _subPath;


        protected Path(string pathName)
        {
            _pathName = pathName.ToLowerInvariant();
            _parentPath = null;
        }

        protected Path(string pathName,IPath parentPath)
        {
            _pathName = pathName.ToLowerInvariant();
            _parentPath = parentPath;
        }

        

        public static IPathBuilder Create(string pathName)
        {
            return new Path(pathName);
        }

        public  IPath Arg(object value)
        {
            this._argValue = value;
            return this;
        }

        public  IPathBuilder SubPath(string name)
        {
            _subPath = new Path(name, this);
            return (IPathBuilder) _subPath;
        }

        public IPath Build()
        {
            if (this._parentPath == null) return this;
            return this._parentPath.Build();
        }

        private string FormatPathAsString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("/").Append(_pathName);
            if (_argValue!=null)
            {
                sb.Append("/");
                sb.Append(_argValue);
                
            }

            if (_subPath!=null)
            {
                sb.Append(_subPath);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return FormatPathAsString();
        }
    }

    
}