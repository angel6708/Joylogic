using System;
using System.Collections;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace Data.Models
{


	[Serializable]
	[DataContract]
     [Table(KeyColumn="key", TableName="global_config" ,IsSnapshot= false  )]
	public partial class GlobalConfig : IModel , INotifyPropertyChanged
    {

        
        private Guid _key;
        private string _ext01;
        private string _ext02;
        private string _ext03;
        private string _ext04;
        private string _ext05;
        private DateTime _createtime;
        private DateTime? _updatetime;
        private Guid? _updateby;
        private Guid? _createby;
        private string _sourceid;
        private string _configgroup;
        private string _configkey;
        private string _configvalue;

		
 
		
		#region Public Properties

        /// <summary>
        /// column:[key],desc:[]
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
        /// column:[create_time],desc:[]
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
        /// column:[update_time],desc:[]
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
        /// column:[update_by],desc:[]
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
        /// column:[create_by],desc:[]
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
        /// column:[source_id],desc:[]
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
        /// column:[config_group],desc:[配置的分组]
        /// </summary>
        [DataMember]
        [Column(Column="config_group" )]
        public virtual string ConfigGroup
        {
            get { return _configgroup; }
            set 
            {
                _configgroup = value;
                OnPropertyChanged("ConfigGroup");
            }
        }
        /// <summary>
        /// column:[config_key],desc:[配置的键]
        /// </summary>
        [DataMember]
        [Column(Column="config_key" )]
        public virtual string ConfigKey
        {
            get { return _configkey; }
            set 
            {
                _configkey = value;
                OnPropertyChanged("ConfigKey");
            }
        }
        /// <summary>
        /// column:[config_value],desc:[配置的值]
        /// </summary>
        [DataMember]
        [Column(Column="config_value" )]
        public virtual string ConfigValue
        {
            get { return _configvalue; }
            set 
            {
                _configvalue = value;
                OnPropertyChanged("ConfigValue");
            }
        }
 
		#endregion 



 #region ORM

         public object GetParam()
        {
           return new {
            
                    Key_ = this.Key,
                    Ext01_ = this.Ext01,
                    Ext02_ = this.Ext02,
                    Ext03_ = this.Ext03,
                    Ext04_ = this.Ext04,
                    Ext05_ = this.Ext05,
                    CreateTime_ = this.CreateTime,
                    UpdateTime_ = this.UpdateTime,
                    UpdateBy_ = this.UpdateBy,
                    CreateBy_ = this.CreateBy,
                    SourceId_ = this.SourceId,
                    ConfigGroup_ = this.ConfigGroup,
                    ConfigKey_ = this.ConfigKey,
                    ConfigValue_ = this.ConfigValue,   //Params
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