using RouteProblem.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteProblem
{
    class School: Station 
    {
       
        public School(int id,string name,string address,double lat,double lon,List<Student> students):base( id,name,0,address,lat,lon,null)
        {
            this.Students = students;
        }

    }
}
