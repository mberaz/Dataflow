using System.Threading.Tasks.Dataflow;

namespace TPLDataFlowDemo
{
    public static class Helper
    {
        public static void LinkWithPredicate<TOut>(
            this ISourceBlock<TOut> source,
            DataflowLinkOptions linkOptions,
            ITargetBlock<TOut> okBlock,
            ITargetBlock<TOut> badBlock,
            Func<TOut, bool> okPredicate
            )
        {
            source.LinkTo(okBlock, linkOptions,
                predicate: ConvertToPredicate(okPredicate));

            source.LinkTo(badBlock, linkOptions,
                predicate: ConvertToNotPredicate(okPredicate));
        }

        private static Predicate<T> ConvertToPredicate<T>(Func<T, bool> func) => x => func(x);

        private static Predicate<T> ConvertToNotPredicate<T>(Func<T, bool> func) => x => !func(x);
    }
}
