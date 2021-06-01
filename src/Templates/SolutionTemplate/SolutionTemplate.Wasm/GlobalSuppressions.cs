// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Wrong Usage", "DF0033:Marks undisposed objects assinged to a property, originated from a method invocation.", Justification = "Disabled for WASM here because .editorconfig rule does not work")]
