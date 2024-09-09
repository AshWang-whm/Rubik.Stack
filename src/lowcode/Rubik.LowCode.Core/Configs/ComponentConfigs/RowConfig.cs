using BootstrapBlazor.Components;

namespace Rubik.LowCode.Core.Configs.ComponentConfigs
{
    public record RowConfig
    {
        public int Index { get; set; }

        public required string Name { get; set; }

        public ItemsPerRow ItemsPerRow { get; set; } = ItemsPerRow.One;

        public string? Style { get; set; }

    }
}
