﻿namespace PixiEditor.ChangeableDocument.Changeables.Interfaces
{
    public interface IReadOnlyStructureMember
    {
        bool IsVisible { get; }
        string Name { get; }
        Guid GuidValue { get; }
        float Opacity { get; }
    }
}