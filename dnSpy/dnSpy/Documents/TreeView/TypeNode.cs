﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnSpy.Contracts.Decompiler;
using dnSpy.Contracts.Documents.TreeView;
using dnSpy.Contracts.Images;
using dnSpy.Contracts.Text;
using dnSpy.Contracts.TreeView;

namespace dnSpy.Documents.TreeView {
	sealed class TypeNode : DocumentTreeNodeData, ITypeNode {
		public override Guid Guid => new Guid(DocumentTreeViewConstants.TYPE_NODE_GUID);
		public override NodePathName NodePathName => new NodePathName(Guid, TypeDef.Namespace + "." + TypeDef.Name);
		public TypeDef TypeDef { get; }
		IMDTokenProvider IMDTokenNode.Reference => TypeDef;
		protected override ImageReference GetIcon(IDotNetImageService dnImgMgr) =>
			dnImgMgr.GetImageReference(TypeDef);
		public override ITreeNodeGroup TreeNodeGroup { get; }

		public TypeNode(ITreeNodeGroup treeNodeGroup, TypeDef type) {
			this.TreeNodeGroup = treeNodeGroup;
			this.TypeDef = type;
		}

		public override void Initialize() => TreeNode.LazyLoading = true;

		public override IEnumerable<ITreeNodeData> CreateChildren() {
			yield return new BaseTypeFolderNode(Context.DocumentTreeView.DocumentTreeNodeGroups.GetGroup(DocumentTreeNodeGroupType.BaseTypeFolderTreeNodeGroupType), TypeDef);
			yield return new DerivedTypesFolderNode(Context.DocumentTreeView.DocumentTreeNodeGroups.GetGroup(DocumentTreeNodeGroupType.DerivedTypesFolderTreeNodeGroupType), TypeDef);

			var hash = TypeDef.GetPropertyAndEventMethods();
			foreach (var m in TypeDef.Methods) {
				if (!hash.Contains(m))
					yield return new MethodNode(Context.DocumentTreeView.DocumentTreeNodeGroups.GetGroup(DocumentTreeNodeGroupType.MethodTreeNodeGroupType), m);
			}
			foreach (var p in TypeDef.Properties)
				yield return new PropertyNode(Context.DocumentTreeView.DocumentTreeNodeGroups.GetGroup(DocumentTreeNodeGroupType.PropertyTreeNodeGroupType), p);
			foreach (var e in TypeDef.Events)
				yield return new EventNode(Context.DocumentTreeView.DocumentTreeNodeGroups.GetGroup(DocumentTreeNodeGroupType.EventTreeNodeGroupType), e);
			foreach (var f in TypeDef.Fields)
				yield return new FieldNode(Context.DocumentTreeView.DocumentTreeNodeGroups.GetGroup(DocumentTreeNodeGroupType.FieldTreeNodeGroupType), f);
			foreach (var t in TypeDef.NestedTypes)
				yield return new TypeNode(Context.DocumentTreeView.DocumentTreeNodeGroups.GetGroup(DocumentTreeNodeGroupType.TypeTreeNodeGroupType), t);
		}

		protected override void Write(ITextColorWriter output, IDecompiler decompiler) =>
			new NodePrinter().Write(output, decompiler, TypeDef, Context.ShowToken);
		public IMethodNode Create(MethodDef method) => Context.DocumentTreeView.Create(method);
		public IPropertyNode Create(PropertyDef property) => Context.DocumentTreeView.Create(property);
		public IEventNode Create(EventDef @event) => Context.DocumentTreeView.Create(@event);
		public IFieldNode Create(FieldDef field) => Context.DocumentTreeView.Create(field);
		public ITypeNode Create(TypeDef type) => Context.DocumentTreeView.CreateNested(type);

		public override FilterType GetFilterType(IDocumentTreeNodeFilter filter) {
			var res = filter.GetResult(TypeDef);
			if (res.FilterType != FilterType.Default)
				return res.FilterType;
			if (Context.Decompiler.ShowMember(TypeDef))
				return FilterType.Visible;
			return FilterType.Hide;
		}
	}
}
