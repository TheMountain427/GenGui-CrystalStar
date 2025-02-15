namespace GenGui_CrystalStar;

public enum BlockFlag
{
    none = 0,
    positive = 1,
    negative = 2,
    width = 3,
    height = 4,
    steps = 5,
    cfg_scale = 6,
    batch_size = 7,
    sd_model = 8,
    sampler_name = 9,
    sampler_index = 10,
    seed = 11,
    subseed = 12,
    subseed_strength = 13,
    outpath_samples = 14,
    outpath_grids = 15,
    prompt_for_display = 16,
    styles = 17,
    seed_resize_from_w = 18,
    seed_resize_from_h = 19,
    n_iter = 20,
    restore_faces = 21,
    tiling = 22,
    do_not_save_samples = 23,
    do_not_save_grid = 24
}

public enum ResultCode
{
    Okay = 1,
    None = 0,
    Invalid = -1,
    NotFound = -2,
    NullItemInput = -3,
    Error = -5,
    DataValidationError = -6,
    AlreadyExists = -7,
    AccessDenied = -8,
    InvalidOperation = -9,
    InvalidData = -10,
    InvalidArgument = -11,
    Timeout = -12,
    Warning = -13,
    Exception = -14,
    UnhandledException = -15,
    Pending = -16,
    Failed = -17,
    DataError = -18,
    GeneralError = -19,
    NotImplemented = -9999,

}


public enum TrimLastComma
{
    True = 1,
    False = 0
}

public enum UseUnderScores
{
    True = 1,
    False = 0
}

public enum TagStyle
{
    Clean = 1,
    Underscore = 2,
    Random = 10
}

public enum OutputType
{
    Positive = 1,
    Api = 2
}

public enum GlobalShuffleSetting
{
    None = 0,
    Full = 1,
    WithinBlocks = 2,
    WholeBlocks = 3,

}

public enum Enabled
{
    Disabled = 0,
    Enabled = 1
}

public enum SelectionScope
{
    Global = 1,
    IndividualBlocks = 2
}

public enum AdjType
{
    All = 0,
    Size = 1,
    Color = 2,
    Other = 3
}