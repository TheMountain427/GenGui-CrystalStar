using System.Text;

namespace GenGui_CrystalStar.Code;

public class FlagResponse<T>
{


    public FlagResponse()
    {

    }

    public FlagResponse(T obj)
    {
        this.Data = obj;
    }

    public FlagResponse(BlockFlag code)
    {
        Success = true;
        this.Code = (int)code;

        switch (code)
        {
            case BlockFlag.none:
                Success = false;
                Flag = "";
                break;
            case BlockFlag.positive:
                Flag = "positive";
                break;
            case BlockFlag.negative:
                Flag = "negative";
                break;
            case BlockFlag.width:
                Flag = "width";
                break;
            case BlockFlag.height:
                Flag = "height";
                break;
            case BlockFlag.steps:
                Flag = "steps";
                break;
            case BlockFlag.cfg_scale:
                Flag = "cfg_scale";
                break;
            case BlockFlag.batch_size:
                Flag = "batch_size";
                break;
            case BlockFlag.sd_model:
                Flag = "sd_model";
                break;
            case BlockFlag.sampler_name:
                Flag = "sampler_name";
                break;
            case BlockFlag.sampler_index:
                Flag = "sampler_index";
                break;
            case BlockFlag.seed:
                Flag = "seed";
                break;
            case BlockFlag.subseed:
                Flag = "subseed";
                break;
            case BlockFlag.subseed_strength:
                Flag = "subseed_strength";
                break;
            case BlockFlag.outpath_samples:
                Flag = "outpath_samples";
                break;
            case BlockFlag.outpath_grids:
                Flag = "outpath_grids";
                break;
            case BlockFlag.prompt_for_display:
                Flag = "prompt_for_display";
                break;
            case BlockFlag.styles:
                Flag = "styles";
                break;
            case BlockFlag.seed_resize_from_w:
                Flag = "seed_resize";
                break;
            case BlockFlag.seed_resize_from_h:
                Flag = "seed_resize";
                break;
            case BlockFlag.n_iter:
                Flag = "n_iter";
                break;
            case BlockFlag.restore_faces:
                Flag = "restore_faces";
                break;
            case BlockFlag.tiling:
                Flag = "tiling";
                break;
            case BlockFlag.do_not_save_samples:
                Flag = "do_not_save_samples";
                break;
            case BlockFlag.do_not_save_grid:
                Flag = "do_not_save_grid";
                break;
            default:
                Success = false;
                Flag = "";
                break;
        }
    }


    public FlagResponse(string flag)
    {
        Success = true;
        this.Flag = flag;

        switch (flag)
        {
            case "positive":
                Code = (int)BlockFlag.positive;
                break;
            case "negative":
                Code = (int)BlockFlag.negative;
                break;
            case "width":
                Code = (int)BlockFlag.width;
                break;
            case "height":
                Code = (int)BlockFlag.height;
                break;
            case "steps":
                Code = (int)BlockFlag.steps;
                break;
            case "cfg_scale":
                Code = (int)BlockFlag.cfg_scale;
                break;
            case "batch_size":
                Code = (int)BlockFlag.batch_size;
                break;
            case "sd_model":
                Code = (int)BlockFlag.sd_model;
                break;
            case "sampler_name":
                Code = (int)BlockFlag.sampler_name;
                break;
            case "sampler_index":
                Code = (int)BlockFlag.sampler_index;
                break;
            case "seed":
                Code = (int)BlockFlag.seed;
                break;
            case "subseed":
                Code = (int)BlockFlag.subseed;
                break;
            case "subseed_strength":
                Code = (int)BlockFlag.subseed_strength;
                break;
            case "outpath_samples":
                Code = (int)BlockFlag.outpath_samples;
                break;
            case "outpath_grids":
                Code = (int)BlockFlag.outpath_grids;
                break;
            case "prompt_for_display":
                Code = (int)BlockFlag.prompt_for_display;
                break;
            case "styles":
                Code = (int)BlockFlag.styles;
                break;
            case "seed_resize":
                Code = (int)BlockFlag.seed_resize_from_w;
                break;
            case "n_iter":
                Code = (int)BlockFlag.n_iter;
                break;
            case "restore_faces":
                Code = (int)BlockFlag.restore_faces;
                break;
            case "tiling":
                Code = (int)BlockFlag.tiling;
                break;
            case "do_not_save_samples":
                Code = (int)BlockFlag.do_not_save_samples;
                break;
            case "do_not_save_grid":
                Code = (int)BlockFlag.do_not_save_grid;
                break;
            default:
                Success = false;
                Code = 0;
                break;
        }
    }

    public bool Success { get; set; } = true;
    public string Flag { get; set; }
    public int Code { get; set; } = 1;
    public T Data { get; set; }
}