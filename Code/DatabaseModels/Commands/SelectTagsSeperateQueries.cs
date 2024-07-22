using GenGui_CrystalStar.Code.Models;
using Microsoft.EntityFrameworkCore;


namespace GenGui_CrystalStar.Code.DatabaseModels.Commands;

public class SelectTagsSeperateQueries
{
    public async static Task<Response<List<GeneratorSettingFramework>>> SelectTagsAsync(GenGuiContext _db,  List<GeneratorSettingFramework> blocksToProcess)
    {
        // Select the Lines
        try
        {
            foreach ( var d in blocksToProcess)
            {
                var query = _db.Tags.AsQueryable();
                query = query.Where(e => e.BlockName == d.BlockName && d.SelectedLineNumbers.Contains(e.LineNumber));

                var result =  await query.ToListAsync();
                d.SelectedLines.AddRange(result);
            }

            return new Response<List<GeneratorSettingFramework>>(blocksToProcess);
        }

        catch (Exception e)
        {
            return new Response<List<GeneratorSettingFramework>>(e.Message, ResultCode.Error);
        }
    }
}