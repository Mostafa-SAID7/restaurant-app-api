export type MenuCategory = 'Appetizers' | 'Mains' | 'Desserts' | 'Drinks' | 'Specials';

export interface MenuItem {
  id: number;
  name: string;
  description: string;
  price: number;
  category: MenuCategory;
  image?: string;
  tags?: string[];        // e.g. ['vegan', 'gluten-free', 'chef-special']
  isAvailable: boolean;
  isFeatured?: boolean;
}
