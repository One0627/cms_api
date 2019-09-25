using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CMS_Entity.Models
{
    public partial class new_TTS_OrderContext : DbContext
    {
        public new_TTS_OrderContext(DbContextOptions<new_TTS_OrderContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TbMenu> TbMenu { get; set; }
        public virtual DbSet<TbPermission> TbPermission { get; set; }
        public virtual DbSet<TbPerRelation> TbPerRelation { get; set; }
        public virtual DbSet<TbRole> TbRole { get; set; }
        public virtual DbSet<TbUser> TbUser { get; set; }
        public virtual DbSet<TbUserRelation> TbUserRelation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TbMenu>(entity =>
            {
                entity.HasKey(e => e.MenuId);

                entity.ToTable("tb_menu");

                entity.Property(e => e.MenuId).HasColumnName("menuId");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.MenuIcon)
                    .HasColumnName("menuIcon")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MenuName)
                    .IsRequired()
                    .HasColumnName("menuName")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.MenuNo).HasColumnName("menuNo");

                entity.Property(e => e.MenuParentId).HasColumnName("menuParentID");

                entity.Property(e => e.MenuUrl)
                    .HasColumnName("menuUrl")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateBy)
                    .HasColumnName("updateBy")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MenuParent)
                    .WithMany(p => p.InverseMenuParent)
                    .HasForeignKey(d => d.MenuParentId)
                    .HasConstraintName("FK_tb_menu_tb_menu");
            });

            modelBuilder.Entity<TbPermission>(entity =>
            {
                entity.HasKey(e => e.PermissId);

                entity.ToTable("tb_permission");

                entity.Property(e => e.PermissId).HasColumnName("permissId");

                entity.Property(e => e.AddState).HasColumnName("addState");

                entity.Property(e => e.DeleteState).HasColumnName("deleteState");

                entity.Property(e => e.ErportState).HasColumnName("erportState");

                entity.Property(e => e.ImportState).HasColumnName("importState");

                entity.Property(e => e.LockState).HasColumnName("lockState");

                entity.Property(e => e.MenuId).HasColumnName("menuId");

                entity.Property(e => e.SearchState).HasColumnName("searchState");

                entity.Property(e => e.UpdateBy)
                    .HasColumnName("updateBy")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateState).HasColumnName("updateState");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.TbPermission)
                    .HasForeignKey(d => d.MenuId)
                    .HasConstraintName("FK_tb_permission_tb_menu");
            });

            modelBuilder.Entity<TbPerRelation>(entity =>
            {
                entity.ToTable("tb_perRelation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.PermissId).HasColumnName("permissId");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.UpdateBy)
                    .HasColumnName("updateBy")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Permiss)
                    .WithMany(p => p.TbPerRelation)
                    .HasForeignKey(d => d.PermissId)
                    .HasConstraintName("FK_tb_perRelation_tb_permission");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TbPerRelation)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_tb_perRelation_tb_role");
            });

            modelBuilder.Entity<TbRole>(entity =>
            {
                entity.HasKey(e => e.RoleId);

                entity.ToTable("tb_role");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("roleName")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.RoleNo)
                    .IsRequired()
                    .HasColumnName("roleNo")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateBy)
                    .HasColumnName("updateBy")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TbUser>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("tb_user");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.UpdateBy)
                    .HasColumnName("updateBy")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("userName")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.UserNo)
                    .IsRequired()
                    .HasColumnName("userNo")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasColumnName("userPassword")
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.UserState)
                    .HasColumnName("userState")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.UserTel)
                    .HasColumnName("userTel")
                    .HasMaxLength(36)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TbUserRelation>(entity =>
            {
                entity.ToTable("tb_userRelation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoleId).HasColumnName("roleId");

                entity.Property(e => e.UpdateBy)
                    .HasColumnName("updateBy")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TbUserRelation)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_tb_userRelation_tb_role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TbUserRelation)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_tb_userRelation_tb_user");
            });
        }
    }
}
