﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using SSIMS.Models;

namespace SSIMS.Database
{
    public class DatabaseInitializer<T> : DropCreateDatabaseAlways<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {

            //Seed data
            InitCollectionPoints(context);
            InitDepartments(context);
            InitItems(context);

            context.SaveChanges();
            //other initializations copy:    static void Init (DatabaseContext context)

            base.Seed(context);
        }

        static void InitCollectionPoints(DatabaseContext context)
        {
            List<CollectionPoint> collectionPoints = new List<CollectionPoint>
            {
                new CollectionPoint("Stationery Store", DateTime.Parse("9:30 AM")),
                new CollectionPoint("Management School", DateTime.Parse("11:00 AM")),
                new CollectionPoint("Medical School", DateTime.Parse("9:30 AM")),
                new CollectionPoint("Engineering School", DateTime.Parse("11:00 AM")),
                new CollectionPoint("Science School", DateTime.Parse("9:30 AM")),
                new CollectionPoint("University Hospital", DateTime.Parse("11:00 AM"))
            };
            foreach (CollectionPoint cp in collectionPoints)
                context.CollectionPoints.Add(cp);

            context.SaveChanges();
        }

        static void InitDepartments(DatabaseContext context)
        {
            List<Department> departments = new List<Department>
            {
                new Department("ARCH", "Architecture"),
                new Department("ARTS", "Arts"),
                new Department("COMM", "Commerce"),
                new Department("CPSC", "Computer Science"),
                new Department("ENGG", "Engineering"),
                new Department("ENGL", "English"),
                new Department("MEDI", "Medicine"),
                new Department("REGR", "Registrar"),
                new Department("SCIE", "Science"),
                new Department("ZOOL", "Zoology"),
            };
            foreach (Department dept in departments)
                context.Departments.Add(dept);
            context.SaveChanges();
        }
        
        static void InitItems(DatabaseContext context)
        {
            List<Item> items = new List<Item>
            {
                new Item("C001","Clip","Clips Double 1\"","Dozen"),
                new Item("C002","Clip","Clips Double 2\"","Dozen"),
                new Item("C003","Clip","Clips Double 3/4\"","Dozen"),
                new Item("C004","Clip","Clips Paper Large","Box"),
                new Item("C005","Clip","Clips Paper Medium","Box"),
                new Item("C006","Clip","Clips Paper Small","Box"),
                new Item("E001","Envelope","Envelope Brown (3\"x6\"","Each"),
                new Item("E002","Envelope","Envelope Brown (3\"x6\" w/Window","Each"),
                new Item("E003","Envelope","Envelope Brown (5\"x7\"","Each"),
                new Item("E004","Envelope","Envelope Brown (5\"x7\" w/Window","Each"),
                new Item("E005","Envelope","Envelope White (3\"x6\"","Each"),
                new Item("E006","Envelope","Envelope White (3\"x6\" w/Window","Each"),
                new Item("E007","Envelope","Envelope White (5\"x7\"","Each"),
                new Item("E008","Envelope","Envelope White (5\"x7\" w/Window","Each"),
                new Item("E020","Eraser","Eraser (hard)","Each"),
                new Item("E021","Eraser","Eraser (soft)","Each"),
                new Item("E030","Exercise","Exercise Book (100 pg)","Each"),
                new Item("E031","Exercise","Exercise Book (120 pg)","Each"),
                new Item("E032","Exercise","Exercise Book A4 Hardcover (100 pg)","Each"),
                new Item("E033","Exercise","Exercise Book A4 Hardcover (120 pg)","Each"),
                new Item("E034","Exercise","Exercise Book A4 Hardcover (200 pg)","Each"),
                new Item("E035","Exercise","Exercise Book Hardcover (100 pg)","Each"),
                new Item("E036","Exercise","Exercise Book Hardcover (120 pg)","Each"),
                new Item("F020","File","File Separator","Set"),
                new Item("F021","File","File-Blue Plain","Each"),
                new Item("F022","File","File-Blue with Logo","Each"),
                new Item("F023","File","File-Brown w/o Logo","Each"),
                new Item("F024","File","File-Brown with Logo","Each"),
                new Item("F031","File","Folder Plastic Blue","Each"),
                new Item("F032","File","Folder Plastic Clear","Each"),
                new Item("F033","File","Folder Plastic Green","Each"),
                new Item("F034","File","Folder Plastic Pink","Each"),
                new Item("F035","File","Folder Plastic Yellow","Each"),
                new Item("H011","Pen","Highlighter Blue","Box"),
                new Item("H012","Pen","Highlighter Green","Box"),
                new Item("H013","Pen","Highlighter Pink","Box"),
                new Item("H014","Pen","Highlighter Yellow","Box"),
                new Item("H031","Puncher","Highlighter Yellow","Box"),


            };
            foreach (Item item in items)
                context.Items.Add(item);
            context.SaveChanges();
        }
    }
}