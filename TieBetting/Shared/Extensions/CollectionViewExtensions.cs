using DevExpress.Maui.CollectionView;

namespace TieBetting.Shared.Extensions
{
    public static class CollectionViewExtensions
    {
        public static void ScrollToToday(this DXCollectionView collectionView, IList<MatchViewModel> matches, int offset)
        {
            if (matches.Any() == false)
            {
                return;
            }

            var match = matches.FirstOrDefault(x => x.Day >= DayProvider.TodayDay);

            if (match == null)
            {
                match = matches.LastOrDefault(x => x.Day < DayProvider.TodayDay);
            }

            if (match != null)
            {
                var index = matches.IndexOf(match);
                index = index <= offset ? index : index + offset;
                collectionView.ScrollTo(Math.Min(index, matches.Count - 1));
            }
        }
    }
}
