﻿using PixiEditor.ChangeableDocument.ChangeInfos.Structure;

namespace PixiEditor.ChangeableDocument.Changes.Structure;

internal class MoveStructureMember_Change : Change
{
    private Guid memberGuid;

    private Guid targetFolderGuid;
    private int targetFolderIndex;

    private Guid originalFolderGuid;
    private int originalFolderIndex;

    [GenerateMakeChangeAction]
    public MoveStructureMember_Change(Guid memberGuid, Guid targetFolder, int targetFolderIndex)
    {
        this.memberGuid = memberGuid;
        this.targetFolderGuid = targetFolder;
        this.targetFolderIndex = targetFolderIndex;
    }

    public override OneOf<Success, Error> InitializeAndValidate(Document document)
    {
        var (member, curFolder) = document.FindChildAndParent(memberGuid);
        var targetFolder = document.FindMember(targetFolderGuid);
        if (member is null || curFolder is null || targetFolder is not Folder)
            return new Error();
        originalFolderGuid = curFolder.GuidValue;
        originalFolderIndex = curFolder.Children.IndexOf(member);
        return new Success();
    }

    private static void Move(Document document, Guid memberGuid, Guid targetFolderGuid, int targetIndex)
    {
        var targetFolder = (Folder)document.FindMemberOrThrow(targetFolderGuid);
        var (member, curFolder) = document.FindChildAndParentOrThrow(memberGuid);

        curFolder.Children.Remove(member);
        targetFolder.Children.Insert(targetIndex, member);
    }

    public override IChangeInfo? Apply(Document target, out bool ignoreInUndo)
    {
        Move(target, memberGuid, targetFolderGuid, targetFolderIndex);
        ignoreInUndo = false;
        return new MoveStructureMember_ChangeInfo() { GuidValue = memberGuid, ParentFromGuid = originalFolderGuid, ParentToGuid = targetFolderGuid };
    }

    public override IChangeInfo? Revert(Document target)
    {
        Move(target, memberGuid, originalFolderGuid, originalFolderIndex);
        return new MoveStructureMember_ChangeInfo() { GuidValue = memberGuid, ParentFromGuid = targetFolderGuid, ParentToGuid = originalFolderGuid };
    }
}
