using GenGui_CrystalStar.Code.Models;
using Microsoft.EntityFrameworkCore;
using GenGui_CrystalStar.Code.DatabaseModels.Commands.Helpers;
using System.Linq.Expressions;

namespace GenGui_CrystalStar.Code.DatabaseModels.Commands;

public class SelectTagsSingleQuery
{
    public static async Task<Response<List<GeneratorSettingFramework>>> SelectTagsWithJoinAsync(GenGuiContext _db,  List<GeneratorSettingFramework> blocksToProcess)
    {
        try
        {
            // Select the Lines with Joins, uh whatever the fuck this is
            var flagJoin = _db.Blocks.AsQueryable();
            var query = from tags in _db.Tags
                        join blocks in flagJoin on tags.BlockName equals blocks.BlockName
                        select tags;

            Expression<Func<Tags, bool>> orPredicate = null;
            foreach ( var currentBlock in blocksToProcess)
            {
                var currentCriteria = currentBlock; // To capture the current variable for the closure
                Expression<Func<Tags, bool>> currentPredicate = e => e.BlockName == currentCriteria.BlockName && currentCriteria.SelectedLineNumbers.Contains(e.LineNumber);

                if (orPredicate == null)
                {
                    orPredicate = currentPredicate;
                }
                else
                {
                    orPredicate = orPredicate.Or(currentPredicate);
                }
            }

            if (orPredicate != null)
            {
                query = query.Where(orPredicate);
            }

            var result = await query.ToListAsync();

            foreach (var currentBlock in blocksToProcess)
            {
                currentBlock.SelectedLines.AddRange(result.Where(x => x.BlockName == currentBlock.BlockName));
            }

            return new Response<List<GeneratorSettingFramework>>(blocksToProcess);
        }

        catch (Exception e)
        {
            return new Response<List<GeneratorSettingFramework>>(e.Message, ResultCode.Error);
        }
    }

    public static async Task<Response<List<GeneratorSettingFramework>>> SelectTagsAsync(GenGuiContext _db,  List<GeneratorSettingFramework> blocksToProcess)
    {
        try
        {
            // Select the Lines with Joins, uh whatever the fuck this is
            var query = _db.Tags.AsQueryable();

            Expression<Func<Tags, bool>> orPredicate = null;
            foreach ( var currentBlock in blocksToProcess)
            {
                var currentCriteria = currentBlock; // To capture the current variable for the closure
                Expression<Func<Tags, bool>> currentPredicate = e => e.BlockName == currentCriteria.BlockName && currentCriteria.SelectedLineNumbers.Contains(e.LineNumber);

                if (orPredicate == null)
                {
                    orPredicate = currentPredicate;
                }
                else
                {
                    orPredicate = orPredicate.Or(currentPredicate);
                }
            }

            if (orPredicate != null)
            {
                query = query.Where(orPredicate);
            }

            var result = await query.ToListAsync();

            foreach (var currentBlock in blocksToProcess)
            {
                currentBlock.SelectedLines.AddRange(result.Where(x => x.BlockName == currentBlock.BlockName));
            }

            return new Response<List<GeneratorSettingFramework>>(blocksToProcess);
        }

        catch (Exception e)
        {
            return new Response<List<GeneratorSettingFramework>>(e.Message, ResultCode.Error);
        }
    }
}

