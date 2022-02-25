﻿using ChangeableDocument.Changeables.Interfaces;

namespace ChangeableDocument.Changeables
{
    internal class Document : IChangeable, IReadOnlyDocument
    {
        public IReadOnlyFolder ReadOnlyStructureRoot => StructureRoot;
        IReadOnlyStructureMember? IReadOnlyDocument.FindMember(Guid guid) => FindMember(guid);
        IReadOnlyList<IReadOnlyStructureMember> IReadOnlyDocument.FindMemberPath(Guid guid) => FindMemberPath(guid);
        IReadOnlyStructureMember IReadOnlyDocument.FindMemberOrThrow(Guid guid) => FindMemberOrThrow(guid);
        (IReadOnlyStructureMember, IReadOnlyFolder) IReadOnlyDocument.FindChildAndParentOrThrow(Guid guid) => FindChildAndParentOrThrow(guid);

        internal Folder StructureRoot { get; set; } = new() { GuidValue = Guid.Empty };

        public StructureMember FindMemberOrThrow(Guid guid) => FindMember(guid) ?? throw new Exception("Could not find member with guid " + guid.ToString());
        public StructureMember? FindMember(Guid guid)
        {
            var list = FindMemberPath(guid);
            return list.Count > 0 ? list[0] : null;
        }

        public (StructureMember, Folder) FindChildAndParentOrThrow(Guid childGuid)
        {
            var path = FindMemberPath(childGuid);
            if (path.Count < 2)
                throw new Exception("Couldn't find child and parent");
            return (path[0], (Folder)path[1]);
        }

        public List<StructureMember> FindMemberPath(Guid guid)
        {
            var list = new List<StructureMember>();
            if (FillMemberPath(StructureRoot, guid, list))
                list.Add(StructureRoot);
            return list;
        }

        private bool FillMemberPath(Folder folder, Guid guid, List<StructureMember> toFill)
        {
            if (folder.GuidValue == guid)
            {
                return true;
            }
            foreach (var member in folder.Children)
            {
                if (member is Layer childLayer && childLayer.GuidValue == guid)
                {
                    toFill.Add(member);
                    return true;
                }
                if (member is Folder childFolder)
                {
                    if (FillMemberPath(childFolder, guid, toFill))
                    {
                        toFill.Add(childFolder);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}