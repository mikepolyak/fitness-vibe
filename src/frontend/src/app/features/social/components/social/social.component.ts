import { Component, OnInit } from '@angular/core';

interface Friend {
  id: number;
  name: string;
  avatar: string;
  level: number;
  isOnline: boolean;
  lastActive: Date;
}

interface Post {
  id: number;
  user: Friend;
  content: string;
  type: 'activity' | 'achievement' | 'milestone' | 'challenge';
  timestamp: Date;
  likes: number;
  comments: number;
  media?: string[];
}

@Component({
  selector: 'app-social',
  templateUrl: './social.component.html',
  styleUrls: ['./social.component.scss']
})
export class SocialComponent implements OnInit {
  activeTab: 'feed' | 'friends' | 'challenges' | 'groups' = 'feed';

  friends: Friend[] = [
    {
      id: 1,
      name: 'Sarah Johnson',
      avatar: 'assets/images/avatars/sarah.jpg',
      level: 25,
      isOnline: true,
      lastActive: new Date()
    },
    {
      id: 2,
      name: 'Mike Chen',
      avatar: 'assets/images/avatars/mike.jpg',
      level: 18,
      isOnline: false,
      lastActive: new Date(Date.now() - 3600000) // 1 hour ago
    },
    {
      id: 3,
      name: 'Emma Wilson',
      avatar: 'assets/images/avatars/emma.jpg',
      level: 32,
      isOnline: true,
      lastActive: new Date()
    }
  ];
  posts: Post[] = [
    {
      id: 1,
      user: {
        id: 1,
        name: 'Sarah Johnson',
        avatar: 'assets/images/avatars/sarah.jpg',
        level: 25,
        isOnline: true,
        lastActive: new Date()
      },
      content: 'Just completed a 5K run! Personal best time 🏃‍♀️',
      type: 'activity',
      timestamp: new Date(Date.now() - 1800000), // 30 minutes ago
      likes: 12,
      comments: 3,
      media: ['assets/images/activities/run-map.jpg']
    },
    {
      id: 2,      user: {
        id: 2,
        name: 'Mike Chen',
        avatar: 'assets/images/avatars/mike.jpg',
        level: 18,
        isOnline: false,
        lastActive: new Date(Date.now() - 3600000)
      },
      content: 'Earned the "Early Bird" badge for completing 5 morning workouts! 🌅',
      type: 'achievement',
      timestamp: new Date(Date.now() - 7200000), // 2 hours ago
      likes: 8,
      comments: 1
    },
    {
      id: 3,      user: {
        id: 3,
        name: 'Emma Wilson',
        avatar: 'assets/images/avatars/emma.jpg',
        level: 32,
        isOnline: true,
        lastActive: new Date()
      },
      content: 'Who wants to join the "Summer Shape-Up" challenge? 30 days of consistent workouts! 💪',
      type: 'challenge',
      timestamp: new Date(Date.now() - 14400000), // 4 hours ago
      likes: 15,
      comments: 6
    }
  ];

  suggestedFriends = [
    {
      id: 4,
      name: 'David Brown',
      avatar: 'assets/images/avatars/david.jpg',
      mutualFriends: 3
    },
    {
      id: 5,
      name: 'Lisa Park',
      avatar: 'assets/images/avatars/lisa.jpg',
      mutualFriends: 2
    }
  ];

  activeGroups = [
    {
      id: 1,
      name: 'Morning Runners Club',
      members: 156,
      image: 'assets/images/groups/runners.jpg'
    },
    {
      id: 2,
      name: 'Yoga Enthusiasts',
      members: 89,
      image: 'assets/images/groups/yoga.jpg'
    }
  ];

  constructor() { }

  ngOnInit(): void {
    // Initialize social data
  }

  setActiveTab(tab: 'feed' | 'friends' | 'challenges' | 'groups'): void {
    this.activeTab = tab;
  }

  likePost(post: Post): void {
    post.likes++;
  }

  sharePost(post: Post): void {
    // Implement share functionality
    console.log('Sharing post:', post);
  }

  commentOnPost(post: Post): void {
    // Implement comment functionality
    console.log('Commenting on post:', post);
  }

  addFriend(friendId: number): void {
    // Implement add friend functionality
    console.log('Adding friend:', friendId);
  }

  joinGroup(groupId: number): void {
    // Implement join group functionality
    console.log('Joining group:', groupId);
  }
}
