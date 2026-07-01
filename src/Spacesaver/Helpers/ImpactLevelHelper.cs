using Microsoft.UI.Xaml.Media;
using Spacesaver.Models;

namespace Spacesaver.Helpers;

public static class ImpactLevelHelper
{
    public static Brush GetAccentBrush(ImpactLevel level) =>
        SemanticColorHelper.GetAccentBrush(SemanticColorHelper.ForImpactLevel(level));

    public static Brush GetTintBrush(ImpactLevel level) =>
        SemanticColorHelper.GetTintBrush(SemanticColorHelper.ForImpactLevel(level));

    public static string GetLabel(ImpactLevel level) =>
        level switch
        {
            ImpactLevel.High => "High impact",
            ImpactLevel.Medium => "Medium impact",
            ImpactLevel.Low => "Low impact",
            _ => "Impact"
        };
}
