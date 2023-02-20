namespace HUMTBot
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.HUMTBotProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.HUMTBotInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // HUMTBotProcessInstaller
            // 
            this.HUMTBotProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.HUMTBotProcessInstaller.Password = null;
            this.HUMTBotProcessInstaller.Username = null;
            // 
            // HUMTBotInstaller
            // 
            this.HUMTBotInstaller.ServiceName = "HUMTBot";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.HUMTBotProcessInstaller,
            this.HUMTBotInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller HUMTBotProcessInstaller;
        private System.ServiceProcess.ServiceInstaller HUMTBotInstaller;
    }
}