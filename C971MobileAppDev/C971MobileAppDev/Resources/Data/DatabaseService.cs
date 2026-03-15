using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using C971MobileAppDev.Resources.Models;

namespace C971MobileAppDev.Resources.Data
{
    public class DatabaseService
    {
        SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "C971MobileAppDev.db");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<Term>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<Assessment>();
        }

        public async Task<List<Term>> GetTermsAsync()
        {
            await Init();
            return await _database.Table<Term>().ToListAsync();
        }

        public async Task<List<Course>> GetCoursesAsync(int termId)
        {
            await Init();
            return await _database.Table<Course>()
                .Where(course => course.TermId == termId)
                .OrderBy(course => course.StartDate)
                .ToListAsync();
        }

        public async Task SaveCourseAsync(Course course)
        {
            await Init();
            if (course.Id != 0)
                await _database.UpdateAsync(course);
            else
                await _database.InsertAsync(course);
        }
        public async Task DeleteCourseAsync(Course course)
        {
            await Init();
            await _database.DeleteAsync(course);
        }

        public async Task SaveTermAsync(Term term)
        {
            await Init();
            if (term.Id != 0)
                await _database.UpdateAsync(term);
            else
                await _database.InsertAsync(term);
        }

        public async Task DeleteTermAsync(Term term)
        {
            await Init();
            await _database.DeleteAsync(term);
        }

        public async Task<List<Assessment>> GetAssessmentsAsync(int courseId)
        {
            await Init();
            return await _database.Table<Assessment>()
                .Where(assessment => assessment.CourseId == courseId)
                .OrderBy(assessment => assessment.DueDate)
                .ToListAsync();
        }

        public async Task SaveAssessmentAsync(Assessment assessment)
        {
            await Init();
            if (assessment.Id != 0)
                await _database.UpdateAsync(assessment);
            else
                await _database.InsertAsync(assessment);
        }
    }
}
