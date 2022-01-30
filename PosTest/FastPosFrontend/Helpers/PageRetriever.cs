using ServiceInterface.Model;
using System;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public class PageRetriever<T>
    {
 

        private Retriever<T> _retriever;
        private RetrieverAsync<T> _retrieverAsync;

        public bool IsAsync { get; set; } = false;
        public PageRetriever(Retriever<T> retriverDelegate)
        {
            _retriever = retriverDelegate;
        }

        public PageRetriever(RetrieverAsync<T> retriever)
        {
            _retrieverAsync = retriever;
            IsAsync = true;
        }

        public Page<T> Retrieve(int pageIndex, int pageSize)
        {
            return _retriever?.Invoke(pageIndex, pageSize);
        }

        public Task<Page<T>> RetrieveAsync(int pageIndex, int pageSize)
        {
            if (!IsAsync) throw new InvalidOperationException($"Can not Call {nameof(RetrieveAsync)} if initialized by Syncronious Retriever");
            
            return _retrieverAsync?.Invoke(pageIndex, pageSize);
        }

        

    }
}
