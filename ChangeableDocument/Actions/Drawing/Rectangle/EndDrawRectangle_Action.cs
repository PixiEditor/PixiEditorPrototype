﻿using ChangeableDocument.Changes;
using ChangeableDocument.Changes.Drawing;

namespace ChangeableDocument.Actions.Drawing.Rectangle
{
    public record class EndDrawRectangle_Action : IEndChangeAction
    {
        bool IEndChangeAction.IsChangeTypeMatching(IChange change)
        {
            return change is DrawRectangle_UpdateableChange;
        }
    }
}