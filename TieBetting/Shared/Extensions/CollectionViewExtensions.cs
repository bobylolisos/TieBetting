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
                collectionView.ScrollTo(Math.Min(index + offset, matches.Count - 1));
            }
        }
    }
}
