namespace Team2LibraryProject_01.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MemberType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "MemberType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MemberType");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "FirstName");
        }
    }
}
