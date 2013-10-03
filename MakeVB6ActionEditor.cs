using Inedo.BuildMaster.Extensibility.Actions;
using Inedo.BuildMaster.Web.Controls;
using Inedo.BuildMaster.Web.Controls.Extensions;
using Inedo.Web.Controls;

namespace Inedo.BuildMasterExtensions.VB6
{
    internal sealed class MakeVB6ActionEditor : ActionEditorBase
    {
        private ValidatingTextBox txtProjectName;
        private ValidatingTextBox txtConditionalCompilationArguments;

        public override bool DisplaySourceDirectory { get { return true; } }
        public override bool DisplayTargetDirectory { get { return true; } }

        public override void BindToForm(ActionBase extension)
        {
            var vb6action = (MakeVB6Action)extension;
            this.txtProjectName.Text = vb6action.ProjectName;
            this.txtConditionalCompilationArguments.Text = vb6action.ConditionalCompilationArguments;
        }

        public override ActionBase CreateFromForm()
        {
            return new MakeVB6Action
            {
                ProjectName = this.txtProjectName.Text,
                ConditionalCompilationArguments = this.txtConditionalCompilationArguments.Text
            };
        }

        protected override void CreateChildControls()
        {
            this.txtProjectName = new ValidatingTextBox
            {
                Required = true
            };

            this.txtConditionalCompilationArguments = new ValidatingTextBox
            {
                ValidationExpression = @"^(\w+=-?\d+)(:\w+=-?\d+)*$"
            };

            this.Controls.Add(
                new FormFieldGroup(
                    "Project",
                    "The name of the project file to be built; e.g. MyProject.vbp or MyProject",
                    false,
                    new StandardFormField("Project Name:", this.txtProjectName)),
                new FormFieldGroup(
                    "Conditional Compilation",
                    "Optionally specify the conditional compliation arguments used by VB6. This field is also defined in the Make tab of the project properties.",
                    true,
                    new StandardFormField("Arguments:", this.txtConditionalCompilationArguments))
            );
        }
    }
}
