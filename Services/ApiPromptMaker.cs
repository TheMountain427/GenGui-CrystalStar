

using GenGui_CrystalStar.Code.Models;

namespace GenGui_CrystalStar.Services;

public class ApiPromptMaker
{
    private List<List<string>> positive { get; set; } = [];
    private List<List<string>> negative { get; set; } = [];
    private List<List<string>> width { get; set; } = [];
    private List<List<string>> height { get; set; } = [];
    private List<List<string>> steps { get; set; } = [];
    private List<List<string>> cfg_scale { get; set; } = [];
    private List<List<string>> batch_size { get; set; } = [];
    private List<List<string>> sd_model { get; set; } = [];
    private List<List<string>> sampler_name { get; set; } = [];
    private List<List<string>> sampler_index { get; set; } = [];
    private List<List<string>> seed { get; set; } = [];
    private List<List<string>> subseed { get; set; } = [];
    private List<List<string>> subseed_strength { get; set; } = [];
    private List<List<string>> outpath_samples { get; set; } = [];
    private List<List<string>> outpath_grids { get; set; } = [];
    private List<List<string>> prompt_for_display { get; set; } = [];
    private List<List<string>> styles { get; set; } = [];
    private List<List<string>> seed_resize_from_w { get; set; } = [];
    private List<List<string>> seed_resize_from_h { get; set; } = [];
    private List<List<string>> n_iter { get; set; } = [];
    private List<List<string>> restore_faces { get; set; } = [];
    private List<List<string>> tiling { get; set; } = [];
    private List<List<string>> do_not_save_samples { get; set; } = [];
    private List<List<string>> do_not_save_grid { get; set; } = [];

    public ApiPromptMaker()
    {

    }

    public string MakeApiPrompt(List<GeneratorSettingFramework> blockSettings, GlobalGenerationSettings globalSettings)
    {
        foreach (var blkSet in blockSettings)
        {
            FillSegments(blkSet);
        }

        var apiPrompt = "";
        if (positive.Any())
        {
            var segment = DoFinalOptions(positive, globalSettings);
            segment = string.Concat("--positive ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (negative.Any())
        {
            var segment = DoFinalOptions(negative, globalSettings);
            segment = string.Concat("--negative ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (width.Any())
        {
            var segment = DoFinalOptions(width, globalSettings);
            segment = string.Concat("--width ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (height.Any())
        {
            var segment = DoFinalOptions(height, globalSettings);
            segment = string.Concat("--height ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (steps.Any())
        {
            var segment = DoFinalOptions(steps, globalSettings);
            segment = string.Concat("--steps ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (cfg_scale.Any())
        {
            var segment = DoFinalOptions(cfg_scale, globalSettings);
            segment = string.Concat("--cfg_scale ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (batch_size.Any())
        {
            var segment = DoFinalOptions(batch_size, globalSettings);
            segment = string.Concat("--batch_size ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (sd_model.Any())
        {
            var segment = DoFinalOptions(sd_model, globalSettings);
            segment = string.Concat("--sd_model ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (sampler_name.Any())
        {
            var segment = DoFinalOptions(sampler_name, globalSettings);
            segment = string.Concat("--sampler_name ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (sampler_index.Any())
        {
            var segment = DoFinalOptions(sampler_index, globalSettings);
            segment = string.Concat("--sampler_index ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (seed.Any())
        {
            var segment = DoFinalOptions(seed, globalSettings);
            segment = string.Concat("--seed ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (subseed.Any())
        {
            var segment = DoFinalOptions(subseed, globalSettings);
            segment = string.Concat("--subseed ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (subseed_strength.Any())
        {
            var segment = DoFinalOptions(subseed_strength, globalSettings);
            segment = string.Concat("--subseed_strength ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (outpath_samples.Any())
        {
            var segment = DoFinalOptions(outpath_samples, globalSettings);
            segment = string.Concat("--outpath_samples ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (outpath_grids.Any())
        {
            var segment = DoFinalOptions(outpath_grids, globalSettings);
            segment = string.Concat("--outpath_grids ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (prompt_for_display.Any())
        {
            var segment = DoFinalOptions(prompt_for_display, globalSettings);
            segment = string.Concat("--prompt_for_display ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (styles.Any())
        {
            var segment = DoFinalOptions(styles, globalSettings);
            segment = string.Concat("--styles ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (seed_resize_from_w.Any())
        {
            var segment = DoFinalOptions(seed_resize_from_w, globalSettings);
            segment = string.Concat("--seed_resize_from_w ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (seed_resize_from_h.Any())
        {
            var segment = DoFinalOptions(seed_resize_from_h, globalSettings);
            segment = string.Concat("--seed_resize_from_h ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (n_iter.Any())
        {
            var segment = DoFinalOptions(n_iter, globalSettings);
            segment = string.Concat("--n_iter ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (restore_faces.Any())
        {
            var segment = DoFinalOptions(restore_faces, globalSettings);
            segment = string.Concat("--restore_faces ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (tiling.Any())
        {
            var segment = DoFinalOptions(tiling, globalSettings);
            segment = string.Concat("--tiling ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (do_not_save_samples.Any())
        {
            var segment = DoFinalOptions(do_not_save_samples, globalSettings);
            segment = string.Concat("--do_not_save_samples ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        if (do_not_save_grid.Any())
        {
            var segment = DoFinalOptions(do_not_save_grid, globalSettings);
            segment = string.Concat("--do_not_save_grid ", "\"", segment, "\"");
            apiPrompt = string.Join(" ", apiPrompt, segment);
        }
        return apiPrompt;
    }

    private string DoFinalOptions(List<List<string>> segment, GlobalGenerationSettings globalSettings)
    {
        var prompt = "";
        var Random = new Random();
        if (globalSettings.ShuffleSetting == GlobalShuffleSetting.WholeBlocks)
        {
            segment = segment.OrderBy(x => Random.Next()).ToList();
        }

        var almostDone = new List<string>();
        foreach (var list in segment)
        {
            almostDone.AddRange(list);
        }

        if (globalSettings.ShuffleSetting == GlobalShuffleSetting.Full)
        {
            almostDone = almostDone.OrderBy(x => Random.Next()).ToList();
        }

        prompt = string.Join(" ", almostDone);

        if (globalSettings.TrimLastComma == TrimLastComma.True)
        {
            if (prompt.Last() == ',' )
                prompt = prompt.TrimEnd(',');
        }

        return prompt;
    }


    private void FillSegments(GeneratorSettingFramework blkSet)
    {

        switch (blkSet.BlockFlag)
        {
            case BlockFlag.positive:
                positive.Add(blkSet.FinalLines);
                break;
            case BlockFlag.negative:
                negative.Add(blkSet.FinalLines);
                break;
            case BlockFlag.width:
                width.Add(blkSet.FinalLines);
                break;
            case BlockFlag.height:
                height.Add(blkSet.FinalLines);
                break;
            case BlockFlag.steps:
                steps.Add(blkSet.FinalLines);
                break;
            case BlockFlag.cfg_scale:
                cfg_scale.Add(blkSet.FinalLines);
                break;
            case BlockFlag.batch_size:
                batch_size.Add(blkSet.FinalLines);
                break;
            case BlockFlag.sd_model:
                sd_model.Add(blkSet.FinalLines);
                break;
            case BlockFlag.sampler_name:
                sampler_name.Add(blkSet.FinalLines);
                break;
            case BlockFlag.sampler_index:
                sampler_index.Add(blkSet.FinalLines);
                break;
            case BlockFlag.seed:
                seed.Add(blkSet.FinalLines);
                break;
            case BlockFlag.subseed:
                subseed.Add(blkSet.FinalLines);
                break;
            case BlockFlag.subseed_strength:
                subseed_strength.Add(blkSet.FinalLines);
                break;
            case BlockFlag.outpath_samples:
                outpath_samples.Add(blkSet.FinalLines);
                break;
            case BlockFlag.outpath_grids:
                outpath_grids.Add(blkSet.FinalLines);
                break;
            case BlockFlag.prompt_for_display:
                prompt_for_display.Add(blkSet.FinalLines);
                break;
            case BlockFlag.styles:
                styles.Add(blkSet.FinalLines);
                break;
            case BlockFlag.seed_resize_from_w:
                seed_resize_from_w.Add(blkSet.FinalLines);
                break;
            case BlockFlag.seed_resize_from_h:
                seed_resize_from_h.Add(blkSet.FinalLines);
                break;
            case BlockFlag.n_iter:
                n_iter.Add(blkSet.FinalLines);
                break;
            case BlockFlag.restore_faces:
                restore_faces.Add(blkSet.FinalLines);
                break;
            case BlockFlag.tiling:
                tiling.Add(blkSet.FinalLines);
                break;
            case BlockFlag.do_not_save_samples:
                do_not_save_samples.Add(blkSet.FinalLines);
                break;
            case BlockFlag.do_not_save_grid:
                do_not_save_grid.Add(blkSet.FinalLines);
                break;
        }
    }
}