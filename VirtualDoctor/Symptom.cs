//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VirtualDoctor
{
    using System;
    using System.Collections.Generic;
    
    public partial class Symptom
    {
        public Symptom()
        {
            this.ConcreteSymptomDiseaseConnections = new HashSet<ConcreteSymptomDiseaseConnection>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string QuickDescription { get; set; }
        public Nullable<int> SymptomCathegory { get; set; }
        public Nullable<int> GeneralizationDegree { get; set; }
    
        public virtual ICollection<ConcreteSymptomDiseaseConnection> ConcreteSymptomDiseaseConnections { get; set; }
        public virtual SymptomCathegory SymptomCathegory1 { get; set; }
    }
}
