﻿using MyClass.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.DAO
{
    public class CategoriesDAO
    {
        private MyDBContext db = new MyDBContext();

        //SELECT * FROM ...
        public List<Categories> getList()
        {
            return db.Caregories.ToList();
        }

        //Select * from cho Index chi voi status 1,2
        public List<Categories> getList(string status = "ALL")//status = 0,1,2
        {
            List<Categories> list = null;
            switch (status)
            {
                case "Index"://1,2
                    {
                        list = db.Caregories.Where(m => m.Status != 0).ToList();
                        break;
                    }
                case "Trash"://0
                    {
                        list = db.Caregories.Where(m => m.Status == 0).ToList();
                        break;
                    }
                default:
                    {
                        list = db.Caregories.ToList();
                        break;
                    }
            }
            return list;
        }

        //Details
        public Categories getRow(int? id)
        {
            if(id == null)
            {
                return null;
            }
            else
            {
                return db.Caregories.Find(id);
            }
        }

        //Tao moi mau tin
        public int Insert(Categories row)
        {
            db.Caregories.Add(row);
            return db.SaveChanges();
        }

        //Cap nhat mau tin
        public int Update(Categories row)
        {
            db.Entry(row).State = EntityState.Modified;
            return db.SaveChanges();
        }

        //Xoa mau tin
        public int Delete(Categories row)
        {
            db.Caregories.Remove(row);
            return db.SaveChanges();
        }
    }
}
