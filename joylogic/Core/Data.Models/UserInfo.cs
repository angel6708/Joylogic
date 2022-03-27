using System;
using System.Collections;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace Data.Models
{


	[Serializable]
	[DataContract]
     [Table(KeyColumn="key", TableName="user_info" ,IsSnapshot= false  )]
	public partial class UserInfo : IModel , INotifyPropertyChanged
    {

        
        private Guid _key;
        private string _username;
        private string _usercode;
        private string _loginname;
        private string _password;
        private string _userrolecode;
        private string _sex;
        private Decimal? _age;
        private DateTime _createtime;
        private DateTime? _updatetime;
        private Guid? _createby;
        private Guid? _updateby;
        private string _ext01;
        private string _ext02;
        private string _ext03;
        private string _ext04;
        private string _ext05;
        private string _sourceid;
        private Guid? _sectionkey;

		
 
		
		#region Public Properties

        /// <summary>
        /// column:[key],desc:[guid]
        /// </summary>
        [DataMember]
        [Column(Column="key" )]
        public virtual Guid Key
        {
            get { return _key; }
            set 
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }
        /// <summary>
        /// column:[user_name],desc:[姓名]
        /// </summary>
        [DataMember]
        [Column(Column="user_name" )]
        public virtual string UserName
        {
            get { return _username; }
            set 
            {
                _username = value;
                OnPropertyChanged("UserName");
            }
        }
        /// <summary>
        /// column:[user_code],desc:[用户编码]
        /// </summary>
        [DataMember]
        [Column(Column="user_code" )]
        public virtual string UserCode
        {
            get { return _usercode; }
            set 
            {
                _usercode = value;
                OnPropertyChanged("UserCode");
            }
        }
        /// <summary>
        /// column:[login_name],desc:[登录名]
        /// </summary>
        [DataMember]
        [Column(Column="login_name" )]
        public virtual string LoginName
        {
            get { return _loginname; }
            set 
            {
                _loginname = value;
                OnPropertyChanged("LoginName");
            }
        }
        /// <summary>
        /// column:[password],desc:[密码]
        /// </summary>
        [DataMember]
        [Column(Column="password" )]
        public virtual string Password
        {
            get { return _password; }
            set 
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
        /// <summary>
        /// column:[user_role_code],desc:[角色代码]
        /// </summary>
        [DataMember]
        [Column(Column="user_role_code" )]
        public virtual string UserRoleCode
        {
            get { return _userrolecode; }
            set 
            {
                _userrolecode = value;
                OnPropertyChanged("UserRoleCode");
            }
        }
        /// <summary>
        /// column:[sex],desc:[性别 男 或女]
        /// </summary>
        [DataMember]
        [Column(Column="sex" )]
        public virtual string Sex
        {
            get { return _sex; }
            set 
            {
                _sex = value;
                OnPropertyChanged("Sex");
            }
        }
        /// <summary>
        /// column:[age],desc:[年龄]
        /// </summary>
        [DataMember]
        [Column(Column="age" )]
        public virtual Decimal? Age
        {
            get { return _age; }
            set 
            {
                _age = value;
                OnPropertyChanged("Age");
            }
        }
        /// <summary>
        /// column:[create_time],desc:[创建时间]
        /// </summary>
        [DataMember]
        [Column(Column="create_time" )]
        public virtual DateTime CreateTime
        {
            get { return _createtime; }
            set 
            {
                _createtime = value;
                OnPropertyChanged("CreateTime");
            }
        }
        /// <summary>
        /// column:[update_time],desc:[更新时间]
        /// </summary>
        [DataMember]
        [Column(Column="update_time" )]
        public virtual DateTime? UpdateTime
        {
            get { return _updatetime; }
            set 
            {
                _updatetime = value;
                OnPropertyChanged("UpdateTime");
            }
        }
        /// <summary>
        /// column:[create_by],desc:[创建者主键]
        /// </summary>
        [DataMember]
        [Column(Column="create_by" )]
        public virtual Guid? CreateBy
        {
            get { return _createby; }
            set 
            {
                _createby = value;
                OnPropertyChanged("CreateBy");
            }
        }
        /// <summary>
        /// column:[update_by],desc:[更新者主键]
        /// </summary>
        [DataMember]
        [Column(Column="update_by" )]
        public virtual Guid? UpdateBy
        {
            get { return _updateby; }
            set 
            {
                _updateby = value;
                OnPropertyChanged("UpdateBy");
            }
        }
        /// <summary>
        /// column:[ext01],desc:[]
        /// </summary>
        [DataMember]
        [Column(Column="ext01" )]
        public virtual string Ext01
        {
            get { return _ext01; }
            set 
            {
                _ext01 = value;
                OnPropertyChanged("Ext01");
            }
        }
        /// <summary>
        /// column:[ext02],desc:[]
        /// </summary>
        [DataMember]
        [Column(Column="ext02" )]
        public virtual string Ext02
        {
            get { return _ext02; }
            set 
            {
                _ext02 = value;
                OnPropertyChanged("Ext02");
            }
        }
        /// <summary>
        /// column:[ext03],desc:[]
        /// </summary>
        [DataMember]
        [Column(Column="ext03" )]
        public virtual string Ext03
        {
            get { return _ext03; }
            set 
            {
                _ext03 = value;
                OnPropertyChanged("Ext03");
            }
        }
        /// <summary>
        /// column:[ext04],desc:[]
        /// </summary>
        [DataMember]
        [Column(Column="ext04" )]
        public virtual string Ext04
        {
            get { return _ext04; }
            set 
            {
                _ext04 = value;
                OnPropertyChanged("Ext04");
            }
        }
        /// <summary>
        /// column:[ext05],desc:[]
        /// </summary>
        [DataMember]
        [Column(Column="ext05" )]
        public virtual string Ext05
        {
            get { return _ext05; }
            set 
            {
                _ext05 = value;
                OnPropertyChanged("Ext05");
            }
        }
        /// <summary>
        /// column:[source_id],desc:[来源主键]
        /// </summary>
        [DataMember]
        [Column(Column="source_id" )]
        public virtual string SourceId
        {
            get { return _sourceid; }
            set 
            {
                _sourceid = value;
                OnPropertyChanged("SourceId");
            }
        }
        /// <summary>
        /// column:[section_key],desc:[隶属科室编码]
        /// </summary>
        [DataMember]
        [Column(Column="section_key" )]
        public virtual Guid? SectionKey
        {
            get { return _sectionkey; }
            set 
            {
                _sectionkey = value;
                OnPropertyChanged("SectionKey");
            }
        }
 
		#endregion 



 #region ORM

         public object GetParam()
        {
           return new {
            
                    Key_ = this.Key,
                    UserName_ = this.UserName,
                    UserCode_ = this.UserCode,
                    LoginName_ = this.LoginName,
                    Password_ = this.Password,
                    UserRoleCode_ = this.UserRoleCode,
                    Sex_ = this.Sex,
                    Age_ = this.Age,
                    CreateTime_ = this.CreateTime,
                    UpdateTime_ = this.UpdateTime,
                    CreateBy_ = this.CreateBy,
                    UpdateBy_ = this.UpdateBy,
                    Ext01_ = this.Ext01,
                    Ext02_ = this.Ext02,
                    Ext03_ = this.Ext03,
                    Ext04_ = this.Ext04,
                    Ext05_ = this.Ext05,
                    SourceId_ = this.SourceId,
                    SectionKey_ = this.SectionKey,   //Params
            };
        }
  
 
        #endregion


         protected virtual void OnPropertyChanged(string p) 
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this , new PropertyChangedEventArgs(p));
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        
		
	}
	
}