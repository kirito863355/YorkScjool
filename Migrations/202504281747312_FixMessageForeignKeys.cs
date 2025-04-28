namespace YorkScjool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixMessageForeignKeys : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Messages", "SenderId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Messages", "ReceiverId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Messages", "Text", c => c.String(nullable: false));
            CreateIndex("dbo.Messages", "SenderId");
            CreateIndex("dbo.Messages", "ReceiverId");
            AddForeignKey("dbo.Messages", "ReceiverId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Messages", "SenderId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "SenderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "ReceiverId", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "ReceiverId" });
            DropIndex("dbo.Messages", new[] { "SenderId" });
            AlterColumn("dbo.Messages", "Text", c => c.String());
            AlterColumn("dbo.Messages", "ReceiverId", c => c.String());
            AlterColumn("dbo.Messages", "SenderId", c => c.String());
        }
    }
}
