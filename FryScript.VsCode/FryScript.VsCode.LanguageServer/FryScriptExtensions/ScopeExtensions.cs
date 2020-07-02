using FryScript.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FryScript.VsCode.LanguageServer.FryScriptExtensions
{
    public static class ScopeExtensions
    {
        public static IEnumerable<ScopeMemberInfo> GetCompletions(this Scope scope, int position)
        {
            if (scope.DeclaringNode == null)
                return scope.Children.First().GetCompletions(position);

            if (!scope.PositionMatch(position))
                return scope.GetAllowedMembers();

            var deeperScope = scope.Children.LastOrDefault();

            if (deeperScope == null)
                return scope.GetAllowedMembers().Where(m => m.AstNode.ParseNode.Span.Location.Position <= position);

            return deeperScope.GetCompletions(position);
        }

        private static bool PositionMatch(this Scope scope, int position)
        {
            return position >= scope.DeclaringNode?.ParseNode.Span.Location.Position
                && position <= scope.DeclaringNode?.ParseNode.Span.EndPosition;
        }
    }
}
