# 🔍 Search & Filter Quick Start Guide

Welcome to the enhanced search and filter functionality! This guide will help you quickly understand and use the new features.

## 🏠 Home Page Overview

The home page now displays different content based on your role:

### For Talents 👨‍💻
- **View**: Job Proposals Feed
- **Search**: Find job opportunities that match your skills
- **Filter**: By hours, skills, companies, and more

### For Customers 🏢
- **View**: Available Talents Feed
- **Search**: Find talents with specific skills and experience
- **Filter**: By location, hourly rate, skills, and categories

## 🎯 Search Features

### Text Search
- **Job Proposals**: Search in proposal names and descriptions
- **Talents**: Search in names, countries, and email addresses
- **Example**: Type "React" to find React-related opportunities or developers

### Filters Available

#### Job Proposals (Talent View)
- **Hours Range**: Set minimum and maximum project hours
- **Skills**: Filter by required technologies
- **Companies**: Filter by specific companies
- **Sort Options**: Name, Total Hours, Skill, Category, Company

#### Talents (Customer View)  
- **Country**: Filter by geographic location
- **Hourly Rate**: Set budget range (€/hour)
- **Skills**: Find talents with specific technical skills
- **Categories**: Filter by talent specialization
- **Sort Options**: Name, Country, Hourly Rate, Category, Email

### Sorting & Pagination
- **Sort Direction**: Ascending or Descending
- **Page Size**: Choose 5, 10, 20, or 50 items per page
- **Navigation**: Easy page navigation with Previous/Next buttons

## 🚀 How to Use

### 1. Basic Search
1. Type your search term in the search box
2. Click "Search" button
3. Results update automatically

### 2. Advanced Filtering
1. Use the filter controls below the search box
2. Set ranges for hours/rates
3. Select specific countries or skills
4. Click "Search" to apply filters

### 3. Sorting Results
1. Choose sort field from dropdown
2. Select ascending or descending order
3. Results re-order automatically

### 4. Pagination
1. Use page size dropdown to control results per page
2. Navigate using Previous/Next buttons
3. Click page numbers for direct navigation

## 💡 Tips for Better Results

### Search Tips
- Use partial words (e.g., "Java" finds "JavaScript")
- Search is case-insensitive
- Try different keywords if no results found

### Filter Tips
- Combine multiple filters for precise results
- Use price ranges to stay within budget
- Filter by country for timezone considerations

### Performance Tips
- Use smaller page sizes for faster loading
- Be specific with search terms
- Apply filters before searching large datasets

## 🎨 Visual Indicators

- **Blue Cards**: Job Proposals
- **Green Cards**: Talent Profiles
- **Icons**: 🎯 Jobs, 👥 Talents, 🔍 Search, 🏢 Company, 🌍 Country, 💰 Rate, ⏱️ Hours

## 📱 Responsive Design

The search interface works on all devices:
- **Desktop**: Full feature set with side-by-side layout
- **Tablet**: Responsive grid layout
- **Mobile**: Stacked layout for easy touch navigation

## 🔧 Technical Details

### API Endpoints Used
- `POST /api/JobProposal/search` - Job proposal search
- `POST /api/Talent/search` - Talent search  
- `GET /api/JobProposal/filter-options` - Available filter options
- `GET /api/Talent/filter-options` - Available filter options

### Authentication
- Job proposal searches require authentication
- Talent searches are publicly accessible
- Filter options are publicly accessible

## 🆘 Troubleshooting

### No Results Found
- Check spelling of search terms
- Try broader search criteria
- Remove some filters to expand results
- Check if you're searching in the right category

### Slow Loading
- Reduce page size
- Be more specific with filters
- Check your internet connection

### Filter Not Working
- Ensure you clicked "Search" after setting filters
- Check that filter values are valid (e.g., positive numbers)
- Try refreshing the page

## 🎉 Next Steps

1. **Explore**: Try different search combinations
2. **Bookmark**: Save useful filter combinations
3. **Connect**: Use the "View Details" buttons to learn more
4. **Apply/Hire**: Take action on interesting opportunities

---

Happy searching! 🚀